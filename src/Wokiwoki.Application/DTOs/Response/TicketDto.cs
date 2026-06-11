namespace Wokiwoki.Application.DTOs.Response
{
	public class TicketDto
	{
		public Guid Id { get; set; }

		public Guid BookingId { get; set; }

		public Guid TicketTypeId { get; set; }

		public int Quantity { get; set; }

		public Guid SessionId { get; set; }

		public string? QrCodeImage { get; set; } = null!;

		public decimal Price { get; set; }

		public bool IsCheckedIn { get; set; } 

		public bool IsActive { get; set; } = true; 

		public WorkshopSessionDto WorkshopSession { get; set; } = null!;

		public BookingDto Booking { get; set; } = null!;
	}
}
