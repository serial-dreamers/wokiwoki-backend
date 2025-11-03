namespace Wokiwoki.Domain.Entities
{
	[Table("booking")]
	public class Booking : BaseAuditableEntity
	{
		public Guid UserId { get; set; }

		public Guid WorkshopId { get; set; }

		public decimal TotalPrice { get; set; }

		public BookingStatus Status { get; set; }

		public Workshop Workshop { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

		public ICollection<Review> Reviews { get; set; } = new List<Review>();

	}
}
