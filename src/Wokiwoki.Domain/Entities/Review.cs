namespace Wokiwoki.Domain.Entities
{
    [Table("review")]
    public class Review : BaseAuditableEntity
    { 
		public Guid WorkshopId { get; set; }

		public Guid BookingId { get; set; }

		public string ImageUrl { get; set; } = null!;

		public string UserId { get; set; } = null!;

		public string Comment { get; set; } = null!;

		public int Rating { get; set; }

		public Workshop Workshop { get; set; } = null!;

		public Booking Booking { get; set; } = null!;

	}
}
