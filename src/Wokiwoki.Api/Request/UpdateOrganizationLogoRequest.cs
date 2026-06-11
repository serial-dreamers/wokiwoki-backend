using System.ComponentModel.DataAnnotations;

namespace Wokiwoki.Api.Request
{
	public class UpdateOrganizationLogoRequest
	{
		[Required]
		public IFormFile LogoFile { get; set; }
	}
}
