using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Features.Workshops.Queries.GetFilterPagedWorkshopsQuery;

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
				.ThenInclude(s => s.WorkshopSessionTickets)
				.Include(w => w.Tags)
				.Include(w => w.Category) 
				.AsQueryable();

			var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
			var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
			// category
			if (request.CategoryId.HasValue)
				query = query.Where(w => w.CategoryId == request.CategoryId.Value);

			// tag
			if (request.TagIds != null && request.TagIds.Any())
				query = query.Where(w => w.Tags.Any(t => request.TagIds.Contains(t.Id)));

			// date type
			if (!string.IsNullOrEmpty(request.DateFilterType))
			{
				if (request.DateFilterType.Equals("today", StringComparison.OrdinalIgnoreCase))
					query = query.Where(w => w.StartTime == vietnamNow.Date);
				else if (request.DateFilterType.Equals("upcoming", StringComparison.OrdinalIgnoreCase))
					query = query.Where(w => w.StartTime > vietnamNow);
			}

			// date range
			if (request.StartDate.HasValue && request.EndDate.HasValue)
				query = query.Where(w => w.StartTime >= request.StartDate && w.EndTime <= request.EndDate);

			// free
			if (request.IsFree.HasValue)
			{
				if (request.IsFree.Value)
					query = query.Where(w => w.WorkshopSessions.All(s => s.WorkshopSessionTickets.All(t => t.Price == 0)));
				else
					query = query.Where(w => w.WorkshopSessions.Any(s => s.WorkshopSessionTickets.Any(t => t.Price > 0)));
			}
			 
			if (request.WorkshopTypeId.HasValue)
				query = query.Where(w => (int)w.ScheduleType == request.WorkshopTypeId);

			var totalCount = await query.CountAsync(cancellationToken);



			var listWorkshop = await query.OrderByDescending(w => w.StartTime)
							.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

			return listWorkshop;
		}

		public async Task<List<Workshop>> GetAllActiveAsync(CancellationToken cancellationToken = default)
		{
			var wL = await _context.Workshops
				.Include(w => w.WorkshopSessions)
				.ThenInclude(s => s.WorkshopSessionTickets)
				.Include(w => w.Tags)
				.Include(w => w.Category) 
				.Where(w => w.IsActive)
				.ToListAsync(cancellationToken);

			return wL;
		}
		public async Task<List<Workshop>> GetByIdActiveAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var workshop = await _context.Workshops
				.Include(w => w.WorkshopSessions)
				.ThenInclude(s => s.WorkshopSessionTickets)
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
		//public async Task<bool> UpdateAsync(Guid id, CancellationToken cancellationToken = default)
		//{
		//	var w = await _context.Workshops.FirstOrDefaultAsync(w => w.Id == id);
		//	w.
		//}

	}
}
