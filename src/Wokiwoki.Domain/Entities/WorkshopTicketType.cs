namespace Wokiwoki.Domain.Entities
{
	[Table("workshop_ticket_type")]
	public class WorkshopTicketType : BaseAuditableEntity
	{
		public string Name { get; set; } = null!;

		public string? Description { get; set; }

		public decimal Price { get; set; }

		public int Quantity { get; set; }

		public int Sold { get; set; }

		public DateTime SaleStart { get; set; }

		public DateTime SaleEnd { get; set; }

		public Guid WorkshopSessionId { get; set; }

		public bool IsActive { get; set; } = true;

		public WorkshopSession WorkshopSession { get; set; } = null!;
	}
}
