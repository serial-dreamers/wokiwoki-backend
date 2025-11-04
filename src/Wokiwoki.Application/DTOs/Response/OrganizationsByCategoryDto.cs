namespace Wokiwoki.Application.DTOs.Response
{
	public class OrganizationsByCategoryDto
	{
		public Guid CategoryId { get; set; }
		public string CategoryName { get; set; } = string.Empty;
		public List<OrganizationDto> Organizations { get; set; } = new();
	}
}

