namespace Wokiwoki.Application.Features.Users.Commands
{
	public class AuthResponseDto
	{
		public string UserId { get; set; } = default!;

		public string Name { get; set; } = default!;

		public string Role { get; set; } = default!;

		public string AccessToken { get; set; } = default!;

		public string RefreshToken { get; set; } = default!;
	}
}
