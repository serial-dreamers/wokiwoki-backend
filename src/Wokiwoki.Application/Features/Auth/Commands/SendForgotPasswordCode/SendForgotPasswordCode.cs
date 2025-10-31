using MediatR;
using System.Security.Cryptography;
using Wokiwoki.Application.Common.Interfaces.Messaging;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.Auth.Commands.SendForgotPasswordCode
{
	public record SendForgotPasswordCodeCommand(string Email) : IRequest<Result>;

	public class SendForgotPasswordCodeCommandHandler : IRequestHandler<SendForgotPasswordCodeCommand, Result>
	{
		private readonly IMessagePublisher _publisher;
		private readonly IRedisCacheService _redisCacheService;
		private readonly IIdentityService _identityService;

		public SendForgotPasswordCodeCommandHandler(
		IMessagePublisher publisher,
		IRedisCacheService redisCacheService,
		IIdentityService identityService)
		{
			_publisher = publisher;
			_redisCacheService = redisCacheService;
			_identityService = identityService;
		}

		public async Task<Result> Handle(SendForgotPasswordCodeCommand request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(request.Email))
				return Result.Failure(new[] { "Email không hợp lệ" });

			var user = await _identityService.FindByEmailAsync(request.Email);
			if (user == null)
				return Result.Failure(new[] { "Email chưa được đăng ký" });

			var normalizedEmail = request.Email.Trim().ToLowerInvariant();
			var rateLimitKey = $"ratelimit:forgot:{normalizedEmail}";

			if (await _redisCacheService.GetAsync(rateLimitKey) != null)
				return Result.Failure(new[] { "Vui lòng đợi trước khi yêu cầu mã khác" });

			var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

			await _redisCacheService.SetAsync($"forgot:{normalizedEmail}", code, TimeSpan.FromMinutes(5));
			await _redisCacheService.SetAsync(rateLimitKey, "1", TimeSpan.FromSeconds(60));

			var emailRequest = new EmailVerificationRequest
			{
				To = request.Email,
				Subject = "Đặt lại mật khẩu",
				Code = code
			};

			await _publisher.PublishAsync(emailRequest, "email-queue", cancellationToken);

			return Result.Success();
		}
	}
}
