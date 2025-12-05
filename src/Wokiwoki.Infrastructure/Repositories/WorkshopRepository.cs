using Microsoft.EntityFrameworkCore;
using QuestPDF.Helpers;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Workshops.Queries.GetFilterPagedWorkshopsQuery;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;
using Wokiwoki.Infrastructure.Data.Extensions;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class WorkshopRepository : BaseRepo<Workshop, Guid>, IWorkshopRepository
	{
		public WorkshopRepository(WokiwokiDbContext context) : base(context)
		{
		}

		public async Task<PaginatedList<Workshop>> SearchAsync(
	 SearchWorkshopQuery request,
	 CancellationToken cancellationToken = default)
		{
			var query = _context.Workshops
				.Include(w => w.WorkshopSessions)
				.Include(w => w.Tags)
				.Include(w => w.Category)
				.Include(w => w.Schedules)
				.ThenInclude(s => s.Tickets)
				.AsQueryable();

			// Title filter - search in workshop title
			if (!string.IsNullOrEmpty(request.Title))
			{
				var titleLower = request.Title.Trim().ToLower();
				query = query.Where(w => w.Title.ToLower().Contains(titleLower));
			}

			// category
			if (request.CategoryId.HasValue)
				query = query.Where(w => w.CategoryId == request.CategoryId.Value);

			// tag
			if (request.TagIds != null && request.TagIds.Any())
				query = query.Where(w => w.Tags.Any(t => request.TagIds.Contains(t.Id)));

			// date type - filter by workshop sessions
			if (!string.IsNullOrEmpty(request.DateFilterType))
			{
				var now = DateTime.UtcNow;
				switch (request.DateFilterType.ToLower())
				{
					case "today":
						var todayStart = now.Date;
						var todayEnd = todayStart.AddDays(1).AddTicks(-1);
						query = query.Where(w => w.WorkshopSessions.Any(s =>
							s.StartTime >= todayStart && s.StartTime <= todayEnd && s.IsActive));
						break;
					case "tomorrow":
						var tomorrowStart = now.Date.AddDays(1);
						var tomorrowEnd = tomorrowStart.AddDays(1).AddTicks(-1);
						query = query.Where(w => w.WorkshopSessions.Any(s =>
							s.StartTime >= tomorrowStart && s.StartTime <= tomorrowEnd && s.IsActive));
						break;
					case "this_weekend":
						var dayOfWeek = (int)now.DayOfWeek;
						var daysToSaturday = (6 - dayOfWeek) % 7;
						var saturdayStart = now.Date.AddDays(daysToSaturday == 0 ? 7 : daysToSaturday);
						var sundayEnd = saturdayStart.AddDays(1).AddDays(1).AddTicks(-1);
						query = query.Where(w => w.WorkshopSessions.Any(s =>
							s.StartTime >= saturdayStart && s.StartTime <= sundayEnd && s.IsActive));
						break;
					case "this_week":
						var startOfWeek = now.AddDays(-(int)now.DayOfWeek + 1).Date;
						var endOfWeek = startOfWeek.AddDays(7).AddTicks(-1);
						query = query.Where(w => w.WorkshopSessions.Any(s =>
							s.StartTime >= startOfWeek && s.StartTime <= endOfWeek && s.IsActive));
						break;
					case "next_week":
						var nextWeekStart = now.AddDays(7 - (int)now.DayOfWeek + 1).Date;
						var nextWeekEnd = nextWeekStart.AddDays(7).AddTicks(-1);
						query = query.Where(w => w.WorkshopSessions.Any(s =>
							s.StartTime >= nextWeekStart && s.StartTime <= nextWeekEnd && s.IsActive));
						break;
					case "this_month":
						var startOfMonth = new DateTime(now.Year, now.Month, 1);
						var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);
						query = query.Where(w => w.WorkshopSessions.Any(s =>
							s.StartTime >= startOfMonth && s.StartTime <= endOfMonth && s.IsActive));
						break;
					case "next_month":
						var startOfNextMonth = new DateTime(now.Year, now.Month, 1).AddMonths(1);
						var endOfNextMonth = startOfNextMonth.AddMonths(1).AddTicks(-1);
						query = query.Where(w => w.WorkshopSessions.Any(s =>
							s.StartTime >= startOfNextMonth && s.StartTime <= endOfNextMonth && s.IsActive));
						break;
				}
			}

			// date range
			if (request.StartDate.HasValue && request.EndDate.HasValue)
				query = query.Where(w => w.WorkshopSessions.Any(s =>
					s.StartTime.Date >= request.StartDate.Value.Date &&
					s.StartTime.Date <= request.EndDate.Value.Date &&
					s.IsActive));

			// free/paid
			if (request.IsFree.HasValue)
			{
				if (request.IsFree.Value)
					query = query.Where(w => w.Schedules.All(s => s.Tickets.All(t => t.Price == 0)));
				else
					query = query.Where(w => w.Schedules.Any(s => s.Tickets.Any(t => t.Price > 0)));
			} 

			// Only return active and published workshops
			query = query.Where(w => w.IsActive && w.Status == WorkshopStatus.Published);

			var totalCount = await query.CountAsync(cancellationToken);

			var listWorkshop = await query.OrderByDescending(w => w.Created)
							.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

			// Filter by location/distance if coordinates provided
			if (request.Latitude.HasValue && request.Longitude.HasValue)
			{
				var radiusKm = request.RadiusInKm ?? 10.0; // Default 10km radius
				var filteredWorkshops = new List<Workshop>();

				foreach (var workshop in listWorkshop.Records)
				{
					double? workshopLat = null;
					double? workshopLng = null;

					// Priority: workshop level coordinates
					if (workshop.Latitude.HasValue && workshop.Longitude.HasValue &&
						workshop.Latitude.Value != 0 && workshop.Longitude.Value != 0)
					{
						workshopLat = workshop.Latitude.Value;
						workshopLng = workshop.Longitude.Value;
					}
					// Fallback to session coordinates
					else if (workshop.WorkshopSessions != null && workshop.WorkshopSessions.Any())
					{
						var sessionWithCoords = workshop.WorkshopSessions
							.FirstOrDefault(s => s.Latitude.HasValue && s.Longitude.HasValue &&
								s.Latitude.Value != 0 && s.Longitude.Value != 0);

						if (sessionWithCoords != null)
						{
							workshopLat = sessionWithCoords.Latitude.Value;
							workshopLng = sessionWithCoords.Longitude.Value;
						}
					}

					if (workshopLat.HasValue && workshopLng.HasValue)
					{
						var distance = CalculateDistance(
							request.Latitude.Value,
							request.Longitude.Value,
							workshopLat.Value,
							workshopLng.Value
						);

						if (distance <= radiusKm)
						{
							filteredWorkshops.Add(workshop);
						}
					}
				}

				// Return filtered results with updated pagination
				return new PaginatedList<Workshop>(
					filteredWorkshops.Skip((request.PageNumber - 1) * request.PageSize)
						.Take(request.PageSize)
						.ToList(),
					filteredWorkshops.Count,
					request.PageNumber,
					request.PageSize
				);
			}

			return listWorkshop;
		}

		// Calculate distance between two coordinates using Haversine formula
		// Returns distance in kilometers
		private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
		{
			const double R = 6371; // Earth radius in kilometers
			var dLat = ToRadians(lat2 - lat1);
			var dLon = ToRadians(lon2 - lon1);

			var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
					Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
					Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

			var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			return R * c;
		}

		private double ToRadians(double degrees)
		{
			return degrees * Math.PI / 180.0;
		}

		public async Task<List<Workshop>> GetAllActiveAsync(CancellationToken cancellationToken = default)
		{
			var wL = await _context.Workshops
				.Include(w => w.Schedules)
				.ThenInclude(s => s.Tickets)
				.Include(w => w.Tags)
				.Include(w => w.Category) 
				.Where(w => w.IsActive)
				.ToListAsync(cancellationToken);

			return wL;
		}
		public async Task<List<Workshop>> GetByIdActiveAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var workshop = await _context.Workshops
				.Include(w => w.Schedules)
				.ThenInclude(s => s.Tickets)
				.Include(w => w.Tags)
				.Include(w => w.Category) 
				.Where(w => w.Id == id && w.IsActive)
				.ToListAsync(cancellationToken);

			return workshop;
		}

		public async Task IncrementLikeCountAsync(Guid workshopId, CancellationToken cancellationToken)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync(
				$"UPDATE workshop SET LikeCount = likecount + 1 WHERE id = {workshopId}", cancellationToken
				);
		}

		public async Task DecrementLikeCountAsync(Guid workshopId, CancellationToken cancellationToken)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync(
				$"UPDATE workshop SET likecount = CASE WHEN likecount > 0 THEN likecount - 1 ELSE 0 END WHERE id = {workshopId}", cancellationToken
				);
		}

		public async Task<bool> CheckWorkshopExistById(Guid workshopId, CancellationToken cancellationToken)
		{
			return await _context.Workshops
				.AnyAsync(ws => ws.Id == workshopId && ws.IsActive, cancellationToken);
		}

		public async Task<Workshop?> GetWorkshopById(Guid workshopId, CancellationToken cancellationToken)
		{
			return await _context.Workshops 
				.Include(wt => wt.Tags)
				.FirstOrDefaultAsync(w => w.Id == workshopId && w.IsActive, cancellationToken);
		}

		public async Task<PaginatedList<Workshop>> GetWorkshopByWorkshopIdAsync(Guid orgId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
		{
			return await _context.Workshops.Where(w => w.OrganizationId == orgId).OrderByDescending(wsc => wsc.Id).ToPaginatedListAsync(pageNumber, pageSize, cancellationToken); 

		}

		public async Task<PaginatedList<Workshop>> GetByOrganizationWithFilterAsync(Guid organizationId, string? title, WorkshopStatus? status, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
		{
			var query = _context.Workshops
				.Where(w => w.OrganizationId == organizationId);

			if (!string.IsNullOrEmpty(title))
				query = query.Where(w => w.Title.Trim().ToLower().Contains(title.Trim().ToLower()));

			if (status.HasValue)
				query = query.Where(w => w.Status == status.Value);

			// Không include WorkshopSessions nữa để tối ưu performance
			// Sessions sẽ được lấy riêng qua GetSessionsByOrganizationAndDateRangeAsync cho Calendar view
			return await query
				.OrderByDescending(w => w.Created)
				.ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
		}

		public async Task<List<WorkshopSession>> GetSessionsByOrganizationAndDateRangeAsync(
			Guid organizationId,
			DateTime startDate,
			DateTime endDate,
			CancellationToken cancellationToken = default)
		{
			return await _context.WorkshopSessions
				.Include(s => s.Workshop)
				.Where(s => s.Workshop.OrganizationId == organizationId
					&& s.StartTime >= startDate
					&& s.StartTime <= endDate
					&& s.IsActive)
				.OrderBy(s => s.StartTime)
				.ToListAsync(cancellationToken);
		}

		public async Task<List<Guid>> GetOrganizationIdsByCategoryAsync(Guid categoryId, int limit, CancellationToken cancellationToken = default)
		{
			return await _context.Workshops
				.Where(w => w.CategoryId == categoryId 
					&& w.IsActive 
					&& w.OrganizationId != null)
				.Select(w => w.OrganizationId!)
				.Distinct()
				.Take(limit)
				.ToListAsync(cancellationToken);
		}

		public async Task<List<Workshop>> GetDiscoverWorkshopsAsync(int limit, CancellationToken cancellationToken = default)
		{
			return await _context.Workshops
				.Where(w => w.IsActive && w.Status == WorkshopStatus.Published)
				.OrderByDescending(w => w.LikeCount) // Sort by popularity first
				.ThenByDescending(w => w.Created) // Then by newest
				.Take(limit)
				.ToListAsync(cancellationToken);
		}

		public async Task<List<Workshop>> GetWorkshopsByTagIdsAsync(List<Guid> tagIds, int limit, CancellationToken cancellationToken = default)
		{
			return await _context.Workshops
				.Where(w => w.IsActive && w.Status == WorkshopStatus.Published)
				.Where(w => w.Tags.Any(t => tagIds.Contains(t.Id)))
				.OrderByDescending(w => w.LikeCount)
				.ThenByDescending(w => w.Created)
				.Take(limit)
				.ToListAsync(cancellationToken);
		}

		public async Task<List<Workshop>> GetWorkshopsByDateRangeAsync(DateTime startDate, DateTime endDate, int limit, CancellationToken cancellationToken = default)
		{
			return await _context.Workshops
				.Where(w => w.IsActive && w.Status == WorkshopStatus.Published)
				.Where(w => w.WorkshopSessions.Any(s =>
					s.StartTime.Date >= startDate &&
					s.StartTime.Date <= endDate &&
					s.IsActive))
				.OrderByDescending(w => w.LikeCount)
				.ThenByDescending(w => w.Created)
				.Take(limit)
				.Distinct()
				.ToListAsync(cancellationToken);
		}

		public async Task<Workshop?> GetWorkshopByIdAsync(Guid workshopId, CancellationToken cancellationToken = default)
		{
			return await _context.Workshops 
				.Include(w => w.Category)
				.Include(w => w.Tags)
				.FirstOrDefaultAsync(w => w.Id == workshopId && w.IsActive);
		}

		//public async Task<bool> UpdateAsync(Guid id, CancellationToken cancellationToken = default)
		//{
		//	var w = await _context.Workshops.FirstOrDefaultAsync(w => w.Id == id);
		//	w.
		//}

		public async Task<PaginatedList<Workshop>> GetWorkshopsByOrganizationAndTimeStatusAsync(
			Guid organizationId,
			int timeStatus,
			int pageNumber,
			int pageSize,
			CancellationToken cancellationToken = default)
		{
			var now = DateTime.UtcNow;
			var query = _context.Workshops
				.Include(w => w.WorkshopSessions)
				.Where(w => w.OrganizationId == organizationId 
					&& w.Status == WorkshopStatus.Published 
					&& w.IsActive);

			// Filter by time-based status based on sessions
			if (timeStatus == 1) // Upcoming - Sắp diễn ra
			{
				// Workshops có sessions với startTime > now
				query = query.Where(w => w.WorkshopSessions.Any(s => 
					s.StartTime > now && s.IsActive));
			}
			else if (timeStatus == 2) // Ongoing - Đang diễn ra
			{
				// Workshops có sessions với startTime <= now && endTime >= now
				query = query.Where(w => w.WorkshopSessions.Any(s => 
					s.StartTime <= now && s.EndTime >= now && s.IsActive));
			}
			else if (timeStatus == 3) // Completed - Đã kết thúc
			{
				// Workshops có tất cả sessions đã kết thúc (max endTime < now)
				// Hoặc workshops có sessions nhưng tất cả đã kết thúc
				query = query.Where(w => w.WorkshopSessions.Any(s => s.IsActive) &&
					w.WorkshopSessions.Where(s => s.IsActive).Max(s => s.EndTime) < now);
			}
			// timeStatus == 0: all - lấy tất cả published workshops

			return await query
				.OrderByDescending(w => w.Created)
				.ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
		}
	}
}
