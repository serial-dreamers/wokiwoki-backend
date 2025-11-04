using Microsoft.AspNetCore.Identity; 

namespace Wokiwoki.Infrastructure.Identity
{
	public class ApplicationUser : IdentityUser
	{
		public string? FullName { get; set; }
		public string? ImageUrl { get; set; }

		public string? Street { get; set; }
		public string? Commune { get; set; }
		public string? Province { get; set; }
		public double? Latitude { get; set; }
		public double? Longitude { get; set; }

		public Guid? OwnedOrganizationId { get; set; }
		public Organization? OwnedOrganization { get; set; }

		public virtual ICollection<UserTagPreference> TagPreferences { get; set; } = new List<UserTagPreference>(); 
	}
}
