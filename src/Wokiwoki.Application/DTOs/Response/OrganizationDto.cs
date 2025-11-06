namespace Wokiwoki.Application.DTOs.Response
{
	public class OrganizationDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		public string? ContactEmail { get; set; }
		public string? ContactPhone { get; set; }
		public string? Street { get; set; }
		public string? Commune { get; set; }
		public string? Province { get; set; }
		public string? LogoUrl { get; set; }
		public int FollowerCount { get; set; }
	}
}
