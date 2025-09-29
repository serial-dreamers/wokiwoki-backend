namespace Wokiwoki.Application.Common.Models
{
	public class AuthResponseDto
	{
		public Guid UserId { get; set; } = default!;

		public string Name { get; set; } = default!;

		public string Role { get; set; } = default!;

		public string AccessToken { get; set; } = default!;

		public string RefreshToken { get; set; } = default!;
	}
}
