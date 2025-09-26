using Microsoft.AspNetCore.Identity; 

namespace Wokiwoki.Infrastructure.Identity
{
	public class ApplicationUser : IdentityUser
	{
		public virtual ICollection<UserTagPreference> TagPreferences { get; set; } = new List<UserTagPreference>();
	}
}
