using Microsoft.AspNetCore.Identity;

namespace Wokiwoki.Infrastructure.Data.Seeder
{
	public static class ApplicationRoleSeeder
	{
		public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
		{
			var roles = new List<IdentityRole>
		{
			new IdentityRole { Name = "Customer", NormalizedName = "CUSTOMER" },
			new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
			new IdentityRole { Name = "Manager", NormalizedName = "MANAGER" },
			new IdentityRole { Name = "Organizer", NormalizedName = "ORGANIZER" },

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
