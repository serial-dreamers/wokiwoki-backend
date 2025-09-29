namespace Wokiwoki.Domain.Entities
{
	[Table("workshop_media")]
	public class WorkshopMedia : BaseAuditableEntity
	{
		public string? ImageUrl { get; set; }

		public MediaType MediaType { get; set; } 

		public Guid UploadedByUserId { get; set; }

		public Guid WorkshopId { get; set; }

		public bool IsActive { get; set; } = true;

		public Workshop Workshop { get; set; } = null!;
	}
}
