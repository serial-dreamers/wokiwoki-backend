namespace Wokiwoki.Domain.Entities
{
	[Table("workshop_session_ticket")]
	public class WorkshopSessionTicket : BaseAuditableEntity
	{
		public Guid WorkshopSessionId { get; set; }
		public Guid? ScheduleTicketId { get; set; }

		public string Name { get; set; } = null!;   
		public decimal Price { get; set; }

		public int Quantity { get; set; }
		public int Sold { get; set; }    
		public bool IsActive { get; set; } = true;

		public WorkshopScheduleTicket? ScheduleTicket { get; set; }
		public WorkshopSession WorkshopSession { get; set; } = null!;
	}
}
