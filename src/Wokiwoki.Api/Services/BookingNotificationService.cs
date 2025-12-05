using Microsoft.AspNetCore.SignalR; 
using Wokiwoki.Api.Hubs;
using Wokiwoki.Application.Common.Interfaces.Services;

namespace Wokiwoki.Api.Services
{
    public class BookingNotificationService : IBookingNotificationService
    {
        private readonly IHubContext<BookingHub> _hubContext;

        public BookingNotificationService(IHubContext<BookingHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyBookingStatusChanged(string bookingId, int status)
        {
            try
            {
                Console.WriteLine($"[BookingNotificationService]  Sending BookingStatusChanged: bookingId={bookingId}, status={status}");
                await _hubContext.Clients.All.SendAsync("BookingStatusChanged", bookingId, status);
                Console.WriteLine($"[BookingNotificationService]  BookingStatusChanged sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BookingNotificationService]  Error sending BookingStatusChanged: {ex.Message}");
                throw;
            }
        }

        public async Task NotifyPaymentSuccess(string bookingId)
        {
            try
            {
                Console.WriteLine($"[BookingNotificationService]  Sending PaymentSuccess: bookingId={bookingId}");
                await _hubContext.Clients.All.SendAsync("PaymentSuccess", bookingId);
                Console.WriteLine($"[BookingNotificationService]  PaymentSuccess sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BookingNotificationService]  Error sending PaymentSuccess: {ex.Message}");
                throw;
            }
        }
    }
}
