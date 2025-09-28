namespace Wokiwoki.Application.Common.Models
{
	public class EmailVerificationResponse
	{
		public string Email { get; set; } = default!;

		public string VerificationCode { get; set; } = null!;
	}
}
