using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.DTOs.Response
{
	public class AuthDto
	{
		public Result Result { get; set; } = null!;

		public string Message { get; set; } = null!; 

		public Data Data { get; set; } = new Data();


	}
	public class Data
	{
		public string AccessToken { get; set; } = default!;

		public string RefreshToken { get; set; } = default!;

		public User User { get; set; } =  new User();
	}

	public class User
	{
		public string Id { get; set; } = string.Empty;
		public string Name { get; set; } = default!;
		public string Email { get; set; } = null!;
		public List<string> Roles { get; set; } = new();

	}
}
