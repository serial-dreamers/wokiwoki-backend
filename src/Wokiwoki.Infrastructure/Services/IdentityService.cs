using Microsoft.AspNetCore.Identity;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models; 

namespace Wokiwoki.Infrastructure.Services
{
	public class IdentityService : IIdentityService
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public IdentityService(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<string?> GetUserNameAsync(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);

			return user?.UserName;
		}


		public async Task<(Result Result, string UserId)> CreateUserAsync(string email, string username, string password, string fullname)
		{
			var user = new ApplicationUser
			{
				UserName = username,
				Email = email,
				FullName = fullname,
				EmailConfirmed = true, 
			};

			var result = await _userManager.CreateAsync(user, password);
			if (!result.Succeeded)
			{
				return (result.ToApplicationResult(), string.Empty);
			}
			var roleResult = await _userManager.AddToRoleAsync(user, "customer");
			if (!roleResult.Succeeded)
			{
				throw new Exception("Failed to assign role.");
			}

			return (result.ToApplicationResult(), user.Id); 
		}

		public async Task<IList<string>> GetRolesByUserIdAsync(string userId)
		{ 
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				throw new Exception("User not found");
			 
			var roles = await _userManager.GetRolesAsync(user);
			return roles;
		}
	}
}
