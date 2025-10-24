namespace Wokiwoki.Domain.Entities
{
	[Table("ticket")]
	public class Ticket : BaseAuditableEntity
	{
		public Guid BookingId { get; set; }

		public Guid TicketTypeId { get; set; }

		public string QrCodeImage { get; set; } = null!;

		public decimal Price { get; set; }

		public bool IsActive { get; set; } = true;

		public WorkshopSessionTicket TicketType { get; set; } = null!;

		public Booking Booking { get; set; } = null!;
	}
}
