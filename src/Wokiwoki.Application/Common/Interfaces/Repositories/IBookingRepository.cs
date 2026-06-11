using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
    public interface IBookingRepository : IBaseRepo<Booking, Guid>
    {
        Task<bool> UpdateBookingStatus(Guid id, BookingStatus status, CancellationToken cancellationToken = default);
        Task<bool> CancleBooking(Guid id, CancellationToken cancellationToken = default);
        Task<bool> CompleteBooking(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ConfirmBooking(Guid id, CancellationToken cancellationToken = default);
        Task<bool> RefundBooking(Guid id, CancellationToken cancellationToken = default);
        Task<PaginatedList<Booking>> GetBookingByMonth(DateTime time, int pageNo, int pageSize, CancellationToken cancellationToken = default);

        
        Task<PaginatedList<Booking>> GetBookingByMonthAndOrganizer(DateTime time, Guid? organizerId, Guid? categoryId, Guid? tagId, int pageNo, int pageSize, CancellationToken cancellationToken);
        
        /// <summary>
        /// Get all bookings for a specific user (only Confirmed and Completed status)
        /// Includes Workshop, Tickets with Sessions, and TicketType information
        /// </summary>
        Task<PaginatedList<Booking>> GetUserBookings(string userId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get dashboard statistics for organizer
        /// </summary>
		Task<OrganizerDashboardDto> GetOrganizerDashboardAsync(
			string userId,
			DateTime? startDate,
			DateTime? endDate,
			Guid? workshopId,
			string chartGroupBy,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Get admin dashboard statistics
		/// </summary>
		Task<AdminDashboardDto> GetAdminDashboardAsync(
			DateTime? startDate,
			DateTime? endDate,
			string chartGroupBy,
			CancellationToken cancellationToken = default);
	}
}
