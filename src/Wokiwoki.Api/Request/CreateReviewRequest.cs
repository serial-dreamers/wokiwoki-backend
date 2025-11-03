namespace Wokiwoki.Api.Request
{
    public class CreateReviewRequest
    {
        public Guid WorkshopId { get; set; }
        public Guid BookingId { get; set; }

        public IFormFile? ImageUrl { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public string Comment { get; set; } = null!;

        public int Rating { get; set; }
    }
}
