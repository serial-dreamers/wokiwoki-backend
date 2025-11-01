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
            // 👉 Bỏ hết \b đi để tránh lỗi boundary, regex này match chuẩn mọi GUID
            var match = Regex.Match(request.Content ?? string.Empty,
                @"[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}");

            if (!match.Success)
            {
                Console.WriteLine($"[ConfirmBooking] ❌ Không tìm thấy BookingId trong content: {request.Content}");
                return false;
            }

            var bookingIdText = match.Value;
            if (!Guid.TryParse(bookingIdText, out Guid bookingId))
            {
                Console.WriteLine($"[ConfirmBooking] ❌ BookingId không hợp lệ: {bookingIdText}");
                return false;
            }

            Console.WriteLine($"[ConfirmBooking] ✅ BookingId trích xuất: {bookingId}");

            // ✅ Xác nhận booking trong repository
            var result = await _repository.ConfirmBooking(bookingId, cancellationToken);
            Console.WriteLine($"[ConfirmBooking] Kết quả xác nhận: {result}");

            return result;
        }

    }
}
