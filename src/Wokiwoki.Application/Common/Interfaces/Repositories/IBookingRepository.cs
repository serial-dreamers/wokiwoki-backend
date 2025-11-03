using Wokiwoki.Application.Common.Models;
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

        Task<PaginatedList<Booking>> GetBookingByMonthAndOrganizer(DateTime time, Guid organizerId, Guid? categoryId, Guid? tagId, int pageNo, int pageSize, CancellationToken cancellationToken = default);
    }
}
