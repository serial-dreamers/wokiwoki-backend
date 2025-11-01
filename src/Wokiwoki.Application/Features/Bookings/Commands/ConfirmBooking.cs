using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Wokiwoki.Application.Features.Bookings.Commands
{
    public sealed record ConfirmBookingCommand(
        string Content,
        string Authorization

        ) : IRequest<bool>;
    public class ConfirmBooking : IRequestHandler<ConfirmBookingCommand, bool>
    {
        private readonly IBookingRepository _repository;
        public ConfirmBooking( IBookingRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
        {
            // ✅ Kiểm tra API key
            var sepayApiKey = Environment.GetEnvironmentVariable("SepayApiKey");

            if (string.IsNullOrWhiteSpace(request.Authorization) ||
                !request.Authorization.StartsWith("Apikey ", StringComparison.OrdinalIgnoreCase))
                return false;

            var apiKey = request.Authorization
                .Replace("Apikey ", "", StringComparison.OrdinalIgnoreCase)
                .Trim();

            if (apiKey != sepayApiKey)
                return false;

            // ✅ Tách BookingId (GUID) từ Content
            var match = Regex.Match(request.Content ?? string.Empty,
                @"\b[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-\b[0-9a-fA-F]{12}\b");

            if (!match.Success)
                return false;

            if (!Guid.TryParse(match.Value, out Guid bookingId))
                return false;

            // ✅ Xác nhận booking
            return await _repository.ConfirmBooking(bookingId, cancellationToken);
        }
    }
}
