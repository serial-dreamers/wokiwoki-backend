using System.Threading.Tasks;

namespace Wokiwoki.Application.Common.Interfaces.Services
{
    public interface IBookingNotificationService
    {
        Task NotifyBookingStatusChanged(string bookingId, int status);
        Task NotifyPaymentSuccess(string bookingId);
    }
}
