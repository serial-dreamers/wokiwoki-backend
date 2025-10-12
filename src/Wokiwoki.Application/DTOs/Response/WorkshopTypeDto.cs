namespace Wokiwoki.Application.DTOs.Response
{
	public class WorkshopTypeDto
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = null!;

		public string? Description { get; set; }

		public string? IconUrl { get; set; }

		public bool IsActive { get; set; } = true;
	}
}
