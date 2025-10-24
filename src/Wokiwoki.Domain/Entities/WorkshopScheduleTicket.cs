namespace Wokiwoki.Domain.Entities
{
	[Table("workshop_schedule_ticket")]
	public class WorkshopScheduleTicket : BaseAuditableEntity
	{
		public Guid WorkshopId { get; set; }
		public Guid? WorkshopScheduleId { get; set; } // null = áp dụng cho toàn workshop

		public string Name { get; set; } = null!;  // "Người lớn", "Trẻ em"
		public decimal Price { get; set; }
		public int MaxQuantity { get; set; } = 20;
		public bool IsActive { get; set; } = true;

		public Workshop Workshop { get; set; } = null!;
		public WorkshopSchedule? WorkshopSchedule { get; set; }

	}
}
