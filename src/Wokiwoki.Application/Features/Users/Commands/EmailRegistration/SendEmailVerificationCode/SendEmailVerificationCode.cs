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

		public SendEmailVerificationCodeCommandHandler(IMessagePublisher publisher)
		{ 
			_publisher = publisher;
		}

		public async Task<Result> Handle(SendEmailVerificationCodeCommand request, CancellationToken cancellationToken)
		{
			var code = new Random().Next(100000, 999999).ToString();

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
