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
    }
}
