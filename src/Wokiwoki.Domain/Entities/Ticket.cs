namespace Wokiwoki.Domain.Entities
{
	[Table("ticket")]
	public class Ticket : BaseAuditableEntity
	{
		public Guid BookingId { get; set; }

		public Guid TicketTypeId { get; set; }

		public int Quantity { get; set; }

		public Guid SessionId { get; set; }

		public string? QrCodeImage { get; set; } = null!;

		public decimal Price { get; set; }

		public bool IsActive { get; set; } = true;

		public WorkshopScheduleTicket TicketType { get; set; } = null!;

		public WorkshopSession WorkshopSession { get; set; } = null!;

		public Booking Booking { get; set; } = null!;
	}
}
