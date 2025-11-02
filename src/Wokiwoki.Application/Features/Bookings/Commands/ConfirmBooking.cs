using MediatR;
using System;
using System.Text.RegularExpressions;
using System.Threading;
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

		public ConfirmBooking(IBookingRepository repository)
		{
			_repository = repository;
		}
        public async Task<bool> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
        {
            // ✅ Lấy Sepay API key từ môi trường
            var sepayApiKey = Environment.GetEnvironmentVariable("SepayApiKey");

            // ✅ Lấy ra API key từ header Authorization
            var apiKey = request.Authorization
                .Replace("Apikey ", "", StringComparison.OrdinalIgnoreCase)
                .Trim();

            Console.WriteLine("Sepay key: " + apiKey);

            // ✅ Regex tổng quát: bắt GUID có hoặc không có dấu '-'
            var match = Regex.Match(request.Content ?? string.Empty,
                @"([0-9a-fA-F]{8}-?[0-9a-fA-F]{4}-?[0-9a-fA-F]{4}-?[0-9a-fA-F]{4}-?[0-9a-fA-F]{12})");

            if (!match.Success)
            {
                Console.WriteLine($"[ConfirmBooking] ❌ Không tìm thấy BookingId trong content: {request.Content}");
                return false;
            }

            // ✅ Lấy chuỗi GUID dạng thô
            var rawId = match.Value;

            // ✅ Nếu thiếu dấu '-', chèn vào đúng chuẩn
            string bookingIdText;
            if (!rawId.Contains('-') && rawId.Length == 32)
            {
                bookingIdText =
                    $"{rawId.Substring(0, 8)}-" +
                    $"{rawId.Substring(8, 4)}-" +
                    $"{rawId.Substring(12, 4)}-" +
                    $"{rawId.Substring(16, 4)}-" +
                    $"{rawId.Substring(20)}";
            }
            else
            {
                bookingIdText = rawId;
            }

            Console.WriteLine($"[ConfirmBooking] ✅ BookingId trích xuất: {bookingIdText}");

            // ✅ Kiểm tra định dạng GUID hợp lệ
            if (!Guid.TryParse(bookingIdText, out Guid bookingId))
            {
                Console.WriteLine($"[ConfirmBooking] ❌ BookingId không hợp lệ: {bookingIdText}");
                return false;
            }

            // ✅ Xác nhận booking trong repository
            var result = await _repository.ConfirmBooking(bookingId, cancellationToken);

            Console.WriteLine($"[ConfirmBooking] ✅ Kết quả xác nhận: {result}");
            return result;
        }

        //public async Task<bool> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
        //{
        //	// ✅ Lấy Sepay API key từ môi trường
        //	var sepayApiKey = Environment.GetEnvironmentVariable("SepayApiKey");

        //	// ✅ Lấy ra API key từ header Authorization
        //	var apiKey = request.Authorization
        //		.Replace("Apikey ", "", StringComparison.OrdinalIgnoreCase)
        //		.Trim();

        //	Console.WriteLine("Sepay key: " + apiKey);

        //	// ✅ Regex tách phần GUID dạng 32 ký tự liền sau dấu '-'
        //	var match = Regex.Match(request.Content ?? string.Empty, @"-([0-9a-fA-F]{32})");

        //	// ✅ Lấy chuỗi 32 ký tự hex
        //	var hexId = match.Groups[1].Value;

        //	// ✅ Chèn dấu gạch đúng chuẩn GUID
        //	var bookingIdText =
        //		$"{hexId.Substring(0, 8)}-" +
        //		$"{hexId.Substring(8, 4)}-" +
        //		$"{hexId.Substring(12, 4)}-" +
        //		$"{hexId.Substring(16, 4)}-" +
        //		$"{hexId.Substring(20)}";

        //	Console.WriteLine($"[ConfirmBooking] BookingId trích xuất: {bookingIdText}");

        //	// ✅ Nếu cần kiểm tra GUID hợp lệ
        //	if (!Guid.TryParse(bookingIdText, out Guid bookingId))
        //	{
        //		Console.WriteLine($"[ConfirmBooking] ❌ BookingId không hợp lệ: {bookingIdText}");
        //		return false;
        //	}

        //	var result = await _repository.ConfirmBooking(bookingId, cancellationToken);
        //	return result;

        //	// ✅ Trả về debug string
        //	//return $"Sepay key: {apiKey}, bookingId: {bookingIdText}";
        //}
    }
}