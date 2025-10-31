namespace Wokiwoki.Api.Request
{
	public class CreateOrganizationRequest
	{
		public string Name { get; set; } = null!;
		public string Description { get; set; } = null!;
		public string ContactEmail { get; set; } = null!;
		public string ContactPhone { get; set; } = null!;
		public string Street { get; set; } = null!;
		public string Commune { get; set; }	 = null!;
		public string Province { get; set; } = null!;
		public IFormFile? LogoFile { get; set; }
	}
}
