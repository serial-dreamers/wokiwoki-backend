namespace Wokiwoki.Domain.Entities
{
	[Table("booking")]
	public class Booking : BaseAuditableEntity
	{
		public string UserId { get; set; }

		public Guid WorkshopId { get; set; }

		public decimal TotalPrice { get; set; }

		public BookingStatus Status { get; set; }

		public string? FullName { get; set; }

		public string? PhoneNumber { get; set; }

		public Workshop Workshop { get; set; } = null!;

        public bool IsActive { get; set; } = true;

		public bool? AllowReminder { get; set; } = false;

		public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

		public ICollection<Review> Reviews { get; set; } = new List<Review>();

	}
}
