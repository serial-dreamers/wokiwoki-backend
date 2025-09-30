using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.Features.Users.Commands;

namespace Wokiwoki.Infrastructure.Services
{
	public class IdentityService : IIdentityService
	{
		private readonly UserManager<ApplicationUser> _userManager; 

		public IdentityService(UserManager<ApplicationUser> userManager )
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

		public async Task<AuthResponseDto> LoginAsync(string username, string password)
		{
			var user = await _userManager.FindByNameAsync(username);
			if (user == null || !await _userManager.CheckPasswordAsync(user, password))
			{
				return new AuthResponseDto
				{ 
					Result = Result.Failure(new[] { "Invalid username or password." }),  
					Message = "Login Failed"
				}; 
			}

			var roles = await _userManager.GetRolesAsync(user); 
			var role = roles.FirstOrDefault() ?? "Customer";

			var authResponse = new AuthResponseDto
			{
				Result = Result.Success(),
				Message = "Login Successfully",
				Data = 
				{
					User = 
					{
						Id = user.Id, 
						Email = user.Email,
						Name = user.FullName, 
						Role = role
					}
				}
			};

			return (authResponse);
		}
	}
}
