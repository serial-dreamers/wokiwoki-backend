using Medo;
using Microsoft.AspNetCore.Identity;

namespace Wokiwoki.Infrastructure.Identity
{
	public static class ApplicationRoleSeeder
	{
		public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
		{
			var roles = new List<IdentityRole>
		{
			new IdentityRole { Name = "customer", NormalizedName = "CUSTOMER" },
			new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
			new IdentityRole { Name = "Manager", NormalizedName = "MANAGER" }
		};

			foreach (var role in roles)
			{
				if (!await roleManager.RoleExistsAsync(role.Name))
				{
					await roleManager.CreateAsync(role);
				}
			}
		}
	}
}
