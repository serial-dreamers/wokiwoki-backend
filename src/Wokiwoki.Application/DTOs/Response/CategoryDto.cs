namespace Wokiwoki.Application.DTOs.Response
{
	public class CategoryDto
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public string Description { get; set; } = string.Empty;

		public string? IconUrl { get; set; }

		public string? ImageUrl { get; set; }

		public List<TagDto> Tags { get; set; } = new();
	}
}
