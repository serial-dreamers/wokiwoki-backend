using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.DTOs.Response
{
	public class WorkshopMediaDto
	{
		public string? ImageUrl { get; set; }

		public MediaType MediaType { get; set; }

		public Guid UploadedByUserId { get; set; }

		public Guid WorkshopId { get; set; }

		public bool IsActive { get; set; } = true;
	}
}
