using MediatR;
using System.Security.Cryptography;
using Wokiwoki.Application.Common.Interfaces.Messaging;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.Auth.Commands.EmailRegistration.SendEmailVerificationCode
{
	public record SendEmailVerificationCodeCommand(string Email) : IRequest<Result>;

	public class SendEmailVerificationCodeCommandHandler : IRequestHandler<SendEmailVerificationCodeCommand, Result>
	{ 
		private readonly IMessagePublisher _publisher;
		private readonly IRedisCacheService _redisCacheService;
		private readonly IIdentityService _identityService;

		public SendEmailVerificationCodeCommandHandler(IMessagePublisher publisher, IRedisCacheService redisCacheService, IIdentityService identityService)
		{ 
			_publisher = publisher;
			_redisCacheService = redisCacheService;
			_identityService = identityService;
		}

		public async Task<Result> Handle(SendEmailVerificationCodeCommand request, CancellationToken cancellationToken)
		{
			if (request == null)
				return Result.Failure(new[] { "Invalid request" }); 

			var email = request.Email; 
			var atIndex = email.IndexOf('@');
			if (atIndex == -1)
			{
				return Result.Failure(new[] { "Email không hợp lệ." });
			}

			var domain = email.Substring(atIndex + 1).ToLowerInvariant();  

			if (domain != "gmail.com" && !domain.EndsWith(".edu"))
			{
				return Result.Failure(new[] { "Chỉ hỗ trợ email Gmail hoặc .edu (email trường học)." });
			}

			var normalizedEmail = request.Email.Trim().ToLowerInvariant();

			var existingUser = await _identityService.FindByEmailAsync(normalizedEmail);
			if (existingUser != null)
			{
				return Result.Failure(new[] { "Email đã được đăng ký. Vui lòng đăng nhập" });
			}

			var rateLimitKey = $"ratelimit:{normalizedEmail}";

			var lastSent = await _redisCacheService.GetAsync(rateLimitKey);
			if (lastSent != null)
			{
				return Result.Failure(new[] { "Vui lòng đợi trước khi yêu cầu mã xác minh khác." });
			}

			var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

			await _redisCacheService.SetAsync($"verify:{normalizedEmail}", code, TimeSpan.FromMinutes(5));

			await _redisCacheService.SetAsync(rateLimitKey, "1", TimeSpan.FromSeconds(60));

			var emailRequest = new EmailVerificationRequest
			{
				To = request.Email,
				Subject = "Xác thực email",
				Code = code
			};

			await _publisher.PublishAsync(emailRequest, "email-queue", cancellationToken);

			return Result.Success();
		}
	}
}
