namespace Wokiwoki.Application.DTOs.Response
{
	public class UserDto
	{
		public string Id { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string? PhoneNumber { get; set; }
		public string? ImageUrl { get; set; }
		public Guid? OwnedOrganizationId { get; set; }
		public string? OrganizationName { get; set; }
		public List<string> Roles { get; set; } = new();
	}
}
