using Microsoft.AspNetCore.SignalR;

namespace Wokiwoki.Api.Hubs
{
    public class BookingHub : Hub
    {
        public async Task SendBookingStatusUpdate(string bookingId, int status)
        {
            await Clients.All.SendAsync("BookingStatusChanged", bookingId, status);
        }

        public async Task SendPaymentSuccess(string bookingId)
        {
            await Clients.All.SendAsync("PaymentSuccess", bookingId);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
