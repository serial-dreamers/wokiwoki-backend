namespace Wokiwoki.Domain.Entities
{
	[Table("workshop_schedule_ticket")]
	public class WorkshopScheduleTicket : BaseAuditableEntity
	{
		public Guid WorkshopScheduleId { get; set; } 

		public string Name { get; set; } = null!;  // "Người lớn", "Trẻ em"
		public decimal Price { get; set; }
		public int MaxQuantity { get; set; } = 20;
		public bool IsActive { get; set; } = true;

		public WorkshopSchedule? WorkshopSchedule { get; set; }
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    }
}
