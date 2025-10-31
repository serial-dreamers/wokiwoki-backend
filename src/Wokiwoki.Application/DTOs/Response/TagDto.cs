namespace Wokiwoki.Application.DTOs.Response
{
	public class TagDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = default!;
		public string? Description { get; set; } = default!;
		public string? IconUrl { get; set; } = default!;
	}
}
