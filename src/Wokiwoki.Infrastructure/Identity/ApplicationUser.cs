using Microsoft.AspNetCore.Identity; 

namespace Wokiwoki.Infrastructure.Identity
{
	public class ApplicationUser : IdentityUser
	{
		public string? FullName { get; set; }

		public Guid? OwnedOrganizationId { get; set; }
		public Organization? OwnedOrganization { get; set; }

		public virtual ICollection<UserTagPreference> TagPreferences { get; set; } = new List<UserTagPreference>(); 
	}
}
