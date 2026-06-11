using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Domain.Enums;
using Wokiwoki.Infrastructure.Data.Extensions;

namespace Wokiwoki.Infrastructure.Repositories
{
    public class BookingRepository : BaseRepo<Booking, Guid>, IBookingRepository
    {
        public BookingRepository(WokiwokiDbContext context) : base(context)
        {
        }

        public new async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Bookings
                .Include(b => b.Workshop)
                .Include(b => b.Tickets)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }
        public async Task<bool> UpdateBookingStatus(Guid id, BookingStatus status, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity != null)
            {
                entity.Status = status;
            }
            await UpdateAsync(id, entity);
            await _context.SaveChangesAsync();
            var updated = await GetByIdAsync(id);
            return updated.Status == status;
        }
        public async Task<bool> CancleBooking(Guid id, CancellationToken cancellationToken = default)
        {
            return await UpdateBookingStatus(id, BookingStatus.Cancelled, cancellationToken);
        }
        public async Task<bool> CompleteBooking(Guid id, CancellationToken cancellationToken = default)
        {
            return await UpdateBookingStatus(id, BookingStatus.Completed, cancellationToken);
        }
        public async Task<bool> ConfirmBooking(Guid id, CancellationToken cancellationToken = default)
        {
            return await UpdateBookingStatus(id, BookingStatus.Confirmed, cancellationToken);
        }
        public async Task<bool> RefundBooking(Guid id, CancellationToken cancellationToken = default)
        {
            return await UpdateBookingStatus(id, BookingStatus.Refunded, cancellationToken);
        }
        public async Task<PaginatedList<Booking>> GetBookingByMonth(
    DateTime time, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Bookings
                .AsNoTracking() // ✅ Không giữ context tracking, giảm nguy cơ ObjectDisposedException
                .Where(b => b.Created.Month == time.Month
                         && b.Created.Year == time.Year
                         && b.Status == BookingStatus.Confirmed)
                .Include(b => b.Tickets);

            // ✅ Thực thi truy vấn ngay trong cùng scope (không deferred)
            return await query.ToPaginatedListAsync(pageNo, pageSize, cancellationToken);
        }

        public async Task<PaginatedList<Booking>> GetBookingByMonthAndOrganizer(DateTime time, Guid? organizerId, Guid? categoryId, Guid? tagId, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var bL = _context.Bookings.Where(b => b.Created.Month == time.Month && b.Created.Year == time.Year && b.Status == BookingStatus.Confirmed).Include(b => b.Tickets).AsQueryable();
            if (organizerId != null)
            {
                bL = bL.Where(b => b.Workshop.OrganizationId == organizerId);
            }
            if (categoryId != null)
            {
                bL = bL.Where(b => b.Tickets.Any(b => b.WorkshopSession.Workshop.CategoryId == categoryId));
            }
            if (tagId != null)
            {
                bL = bL.Where(b => b.Tickets.Any(b => b.WorkshopSession.Workshop.Tags.All(t => t.Id == tagId)));
            }
            var result = await bL.ToPaginatedListAsync(pageNo, pageSize, cancellationToken);
            return result;
        }

        public async Task<PaginatedList<Booking>> GetUserBookings(string userId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            // Only get Confirmed (1) and Completed (3) bookings
            // Include all necessary navigation properties
            var query = _context.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == userId
                    && (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed))
                .Include(b => b.Workshop)
                    .ThenInclude(w => w.Organization)
                .Include(b => b.Workshop)
                    .ThenInclude(w => w.Category)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.WorkshopSession)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.TicketType)
                .OrderByDescending(b => b.Created);

        return await query.ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public async Task<OrganizerDashboardDto> GetOrganizerDashboardAsync(
        string userId,
        DateTime? startDate,
        DateTime? endDate,
        Guid? workshopId,
        string chartGroupBy,
        CancellationToken cancellationToken = default)
    {
        const decimal PLATFORM_FEE_PERCENTAGE = 0.05m; // Platform takes 5%, organization gets 95%
        const decimal ORGANIZATION_REVENUE_PERCENTAGE = 0.95m; // Organization gets 95%

        // Get organizer's organization IDs
        var organizationIds = await _context.Organizations
            .Where(o => o.OwnerId == userId)
            .Select(o => o.Id)
            .ToListAsync(cancellationToken);

        if (!organizationIds.Any())
            return new OrganizerDashboardDto();

        // Base query for bookings (Confirmed and Completed only)
        var bookingsQuery = _context.Bookings
            .AsNoTracking()
            .Include(b => b.Workshop)
            .Include(b => b.Tickets)
                .ThenInclude(t => t.WorkshopSession)
            .Include(b => b.Tickets)
                .ThenInclude(t => t.TicketType)
            .Where(b => organizationIds.Contains(b.Workshop.OrganizationId))
            .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed);

        // Apply filters
        if (startDate.HasValue)
        {
            var filterStartDate = startDate.Value.Kind == DateTimeKind.Utc
                ? startDate.Value
                : DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);
            bookingsQuery = bookingsQuery.Where(b => b.Created >= filterStartDate);
        }

        if (endDate.HasValue)
        {
            var filterEndDate = endDate.Value.Kind == DateTimeKind.Utc
                ? endDate.Value
                : DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);
            bookingsQuery = bookingsQuery.Where(b => b.Created <= filterEndDate);
        }

        if (workshopId.HasValue)
        {
            bookingsQuery = bookingsQuery.Where(b => b.WorkshopId == workshopId.Value);
        }

        var bookings = await bookingsQuery.ToListAsync(cancellationToken);

        // Calculate summary statistics
        var summary = new DashboardSummaryDto
        {
            TotalWorkshops = await _context.Workshops
                .Where(w => organizationIds.Contains(w.OrganizationId))
                .Where(w => w.IsActive)
                .CountAsync(cancellationToken),

            UpcomingWorkshops = await _context.WorkshopSessions
                .Where(ws => organizationIds.Contains(ws.Workshop.OrganizationId))
                .Where(ws => ws.StartTime > DateTime.UtcNow)
                .Select(ws => ws.WorkshopId)
                .Distinct()
                .CountAsync(cancellationToken),

            TotalTicketsSold = bookings.SelectMany(b => b.Tickets).Sum(t => t.Quantity),
            TotalRevenue = bookings.SelectMany(b => b.Tickets).Sum(t => t.Price * t.Quantity),
            TotalCheckedIn = bookings.SelectMany(b => b.Tickets).Count(t => t.IsCheckedIn),
            TotalPendingCheckIn = bookings.SelectMany(b => b.Tickets).Count(t => !t.IsCheckedIn)
        };
        summary.OrganizationRevenue = summary.TotalRevenue * ORGANIZATION_REVENUE_PERCENTAGE;

        // Generate revenue chart data
        var revenueChart = GenerateRevenueChart(bookings, chartGroupBy, ORGANIZATION_REVENUE_PERCENTAGE);

        // Generate workshop details
        var workshopDetails = await GenerateWorkshopDetails(
            bookings,
            organizationIds,
            ORGANIZATION_REVENUE_PERCENTAGE,
            cancellationToken);

			return new OrganizerDashboardDto
			{
				Summary = summary,
				RevenueChart = revenueChart,
				WorkshopDetails = workshopDetails
			};
		}

		public async Task<AdminDashboardDto> GetAdminDashboardAsync(
			DateTime? startDate,
			DateTime? endDate,
			string chartGroupBy,
			CancellationToken cancellationToken = default)
		{
			const decimal PLATFORM_FEE_PERCENTAGE = 0.05m; // Platform takes 5%

			// Base query for bookings (Confirmed and Completed only)
			var bookingsQuery = _context.Bookings
				.AsNoTracking()
				.Include(b => b.Workshop)
					.ThenInclude(w => w.Organization)
				.Include(b => b.Tickets)
				.Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed);

			// Apply date filters with UTC conversion
			if (startDate.HasValue)
			{
				var filterStartDate = startDate.Value.Kind == DateTimeKind.Utc
					? startDate.Value
					: DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);
				bookingsQuery = bookingsQuery.Where(b => b.Created >= filterStartDate);
			}

			if (endDate.HasValue)
			{
				var filterEndDate = endDate.Value.Kind == DateTimeKind.Utc
					? endDate.Value
					: DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);
				bookingsQuery = bookingsQuery.Where(b => b.Created <= filterEndDate);
			}

			var bookings = await bookingsQuery.ToListAsync(cancellationToken);

			// Calculate summary
			var totalRevenue = bookings.SelectMany(b => b.Tickets).Sum(t => t.Price * t.Quantity);
			var platformRevenue = totalRevenue * PLATFORM_FEE_PERCENTAGE;

			var summary = new AdminDashboardSummaryDto
			{
				TotalUsers = await _context.Users.CountAsync(cancellationToken),
				TotalOrganizations = await _context.Organizations.CountAsync(cancellationToken),
				PendingOrganizations = await _context.Organizations
					.Where(o => o.Status == OrganizationStatus.Pending)
					.CountAsync(cancellationToken),
				TotalWorkshops = await _context.Workshops.Where(w => w.IsActive).CountAsync(cancellationToken),
				PendingWorkshops = await _context.Workshops
					.Where(w => w.Status == WorkshopStatus.PendingReview)
					.CountAsync(cancellationToken),
				TotalTicketsSold = bookings.SelectMany(b => b.Tickets).Sum(t => t.Quantity),
				TotalRevenue = totalRevenue,
				PlatformRevenue = platformRevenue
			};

			// Generate revenue chart
			var revenueChart = GenerateAdminRevenueChart(bookings, chartGroupBy, PLATFORM_FEE_PERCENTAGE);

			// Get top workshops
			var topWorkshops = await GenerateTopWorkshopsAsync(bookings, 10, cancellationToken);

			return new AdminDashboardDto
			{
				Summary = summary,
				RevenueChart = revenueChart,
				TopWorkshops = topWorkshops
			};
		}

		private List<AdminRevenueChartDataDto> GenerateAdminRevenueChart(
			List<Booking> bookings,
			string groupBy,
			decimal platformFeePercentage)
		{
			var chartData = new List<AdminRevenueChartDataDto>();

			if (!bookings.Any())
				return chartData;

			IEnumerable<IGrouping<string, Booking>> groupedData = groupBy.ToLower() switch
			{
				"week" => bookings.GroupBy(b => $"W{GetWeekOfYear(b.Created)} {b.Created:yyyy}"),
				"month" => bookings.GroupBy(b => b.Created.ToString("MM/yyyy")),
				_ => bookings.GroupBy(b => b.Created.ToString("dd/MM/yyyy")) // day
			};

			foreach (var group in groupedData.OrderBy(g => g.Key))
			{
				var totalRevenue = group.SelectMany(b => b.Tickets).Sum(t => t.Price * t.Quantity);
				var ticketsSold = group.SelectMany(b => b.Tickets).Sum(t => t.Quantity);
				var workshopsCount = group.Select(b => b.WorkshopId).Distinct().Count();

				chartData.Add(new AdminRevenueChartDataDto
				{
					Date = group.Key,
					TotalRevenue = totalRevenue,
					PlatformRevenue = totalRevenue * platformFeePercentage,
					TicketsSold = ticketsSold,
					WorkshopsCount = workshopsCount
				});
			}

			return chartData;
		}

		private async Task<List<AdminWorkshopStatDto>> GenerateTopWorkshopsAsync(
			List<Booking> bookings,
			int limit,
			CancellationToken cancellationToken)
		{
			const decimal PLATFORM_FEE_PERCENTAGE = 0.05m;

			var workshopGroups = bookings
				.GroupBy(b => b.WorkshopId)
				.OrderByDescending(g => g.SelectMany(b => b.Tickets).Sum(t => t.Price * t.Quantity))
				.Take(limit);

			var details = new List<AdminWorkshopStatDto>();

			foreach (var group in workshopGroups)
			{
				var workshop = await _context.Workshops
					.AsNoTracking()
					.Include(w => w.Organization)
					.FirstOrDefaultAsync(w => w.Id == group.Key, cancellationToken);

				if (workshop == null) continue;

				var tickets = group.SelectMany(b => b.Tickets).ToList();
				var totalRevenue = tickets.Sum(t => t.Price * t.Quantity);

				details.Add(new AdminWorkshopStatDto
				{
					WorkshopId = workshop.Id,
					WorkshopTitle = workshop.Title,
					WorkshopImageUrl = workshop.ImageUrl,
					OrganizationName = workshop.Organization.Name,
					TotalTicketsSold = tickets.Sum(t => t.Quantity),
					TotalRevenue = totalRevenue,
					PlatformRevenue = totalRevenue * PLATFORM_FEE_PERCENTAGE,
					Status = (int)workshop.Status
				});
			}

			return details;
		}

		private List<RevenueChartDataDto> GenerateRevenueChart(
        List<Booking> bookings,
        string groupBy,
        decimal organizationRevenuePercentage)
    {
        var chartData = new List<RevenueChartDataDto>();

        if (!bookings.Any())
            return chartData;

        IEnumerable<IGrouping<string, Booking>> groupedData = groupBy.ToLower() switch
        {
            "week" => bookings.GroupBy(b => $"W{GetWeekOfYear(b.Created)} {b.Created:yyyy}"),
            "month" => bookings.GroupBy(b => b.Created.ToString("MM/yyyy")),
            _ => bookings.GroupBy(b => b.Created.ToString("dd/MM/yyyy")) // day
        };

        foreach (var group in groupedData.OrderBy(g => g.Key))
        {
            var totalRevenue = group.SelectMany(b => b.Tickets).Sum(t => t.Price * t.Quantity);
            var ticketsSold = group.SelectMany(b => b.Tickets).Sum(t => t.Quantity);

            chartData.Add(new RevenueChartDataDto
            {
                Date = group.Key,
                Revenue = totalRevenue,
                OrganizationRevenue = totalRevenue * organizationRevenuePercentage, // Organization gets 95%
                TicketsSold = ticketsSold
            });
        }

        return chartData;
    }

    private async Task<List<WorkshopRevenueDetailDto>> GenerateWorkshopDetails(
        List<Booking> bookings,
        List<Guid> organizationIds,
        decimal organizationRevenuePercentage,
        CancellationToken cancellationToken)
    {
        var workshopGroups = bookings.GroupBy(b => b.WorkshopId);
        var details = new List<WorkshopRevenueDetailDto>();

        foreach (var group in workshopGroups)
        {
            var workshop = await _context.Workshops
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == group.Key, cancellationToken);

            if (workshop == null) continue;

            var tickets = group.SelectMany(b => b.Tickets).ToList();
            var totalRevenue = tickets.Sum(t => t.Price * t.Quantity);

            var latestSession = await _context.WorkshopSessions
                .Where(ws => ws.WorkshopId == group.Key)
                .OrderByDescending(ws => ws.StartTime)
                .FirstOrDefaultAsync(cancellationToken);

            details.Add(new WorkshopRevenueDetailDto
            {
                WorkshopId = workshop.Id,
                WorkshopTitle = workshop.Title,
                WorkshopImageUrl = workshop.ImageUrl,
                TotalSessions = await _context.WorkshopSessions
                    .CountAsync(ws => ws.WorkshopId == group.Key, cancellationToken),
                TotalTicketsSold = tickets.Sum(t => t.Quantity),
                TotalRevenue = totalRevenue,
                OrganizationRevenue = totalRevenue * organizationRevenuePercentage, // Organization gets 95%
                TicketsCheckedIn = tickets.Count(t => t.IsCheckedIn),
                TicketsPendingCheckIn = tickets.Count(t => !t.IsCheckedIn),
                LatestSessionDate = latestSession?.StartTime
            });
        }

        return details.OrderByDescending(d => d.TotalRevenue).ToList();
    }

    private static int GetWeekOfYear(DateTime date)
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        return culture.Calendar.GetWeekOfYear(date, 
            System.Globalization.CalendarWeekRule.FirstDay, 
            DayOfWeek.Monday);
    }
}
}