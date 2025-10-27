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
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        public bool IsActive { get; set; } = true;

	}
}
