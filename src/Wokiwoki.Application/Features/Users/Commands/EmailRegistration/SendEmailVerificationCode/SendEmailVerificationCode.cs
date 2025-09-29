using MediatR;
using Wokiwoki.Application.Common.Interfaces.Messaging;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.Users.Commands.EmailRegistration.SendEmailVerificationCode
{
	public record SendEmailVerificationCodeCommand(string Email) : IRequest<Result>;

	public class SendEmailVerificationCodeCommandHandler : IRequestHandler<SendEmailVerificationCodeCommand, Result>
	{ 
		private readonly IMessagePublisher _publisher;
		private readonly IRedisCacheService _redisCacheService;

		public SendEmailVerificationCodeCommandHandler(IMessagePublisher publisher, IRedisCacheService redisCacheService)
		{ 
			_publisher = publisher;
			_redisCacheService = redisCacheService;
		}

		public async Task<Result> Handle(SendEmailVerificationCodeCommand request, CancellationToken cancellationToken)
		{
			if (request == null)
				return Result.Failure(new[] { "Invalid request" });

			var code = new Random().Next(100000, 999999).ToString();

			await _redisCacheService.SetAsync($"{request.Email}", code, TimeSpan.FromMinutes(5));

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
