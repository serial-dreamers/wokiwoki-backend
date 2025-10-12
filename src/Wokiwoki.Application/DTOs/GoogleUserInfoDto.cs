namespace Wokiwoki.Application.DTOs
{
	public class GoogleUserInfoDto
	{ 
		public string? Email { get; set; } 
		public string? FullName { get; set; }
		public string? Picture { get; set; }
		public string ProviderKey { get; set; } = string.Empty;
	}
}
