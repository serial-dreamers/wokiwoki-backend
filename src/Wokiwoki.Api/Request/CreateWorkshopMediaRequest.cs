namespace Wokiwoki.Api.Request
{
	public class CreateWorkshopMediaRequest
	{
		public Guid WorkshopId { get; set; }
		public IFormFile? LogoFile { get; set; }
	}
}
