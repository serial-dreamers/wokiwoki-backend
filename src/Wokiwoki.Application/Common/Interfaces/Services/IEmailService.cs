using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Common.Interfaces.Services
{
	public interface IEmailService
	{
		Task SendEmailVerificationCode(EmailVerificationRequest message);
	}
}
