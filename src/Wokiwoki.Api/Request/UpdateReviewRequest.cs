namespace Wokiwoki.Api.Request
{
    public class UpdateReviewRequest
    {
        public string Comment { get; set; } = null!;

        public int Rating { get; set; }

        public IFormFile? LogoFile { get; set; } = null;
    }
}
