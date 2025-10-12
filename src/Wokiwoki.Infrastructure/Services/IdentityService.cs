using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;

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


		public async Task<(Result Result, string UserId)> CreateUserAsync(string email, string password, string fullname)
		{
			var user = new ApplicationUser
			{
				UserName = email,
				Email = email,
				FullName = fullname,
				EmailConfirmed = true,
			};

			var result = await _userManager.CreateAsync(user, password);
			if (!result.Succeeded)
			{
				return (result.ToApplicationResult(), string.Empty);
			}
			var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
			if (!roleResult.Succeeded)
			{
				throw new Exception("Failed to assign role.");
			}

			return (result.ToApplicationResult(), user.Id);
		}

		public async Task<IEnumerable<string>> GetRolesByUserIdAsync(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				throw new Exception("User not found");

			var roles = await _userManager.GetRolesAsync(user);
			return roles;
		}

		public async Task<AuthDto> LoginAsync(string email, string password)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null || !await _userManager.CheckPasswordAsync(user, password))
			{
				return new AuthDto
				{
					Result = Result.Failure(new[] { "Invalid username or password." }),
					Message = "Login Failed"
				};
			}

			var roles = await _userManager.GetRolesAsync(user);

			var authResponse = new AuthDto
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
						Roles = roles.ToList(),
					}
				}
			};

			return (authResponse);
		}  

		public async Task<AuthDto?> LoginGoogleConfirm(string providerKey, string email, string fullName)
		{
			var provider = "Google";

			// 1️⃣ Tìm user theo External Login
			var user = await _userManager.FindByLoginAsync(provider, providerKey);

			if (user == null)
			{ 
				var existingUser = await _userManager.FindByEmailAsync(email);

				if (existingUser == null)
				{ 
					var randomPassword = Guid.NewGuid().ToString("N").Substring(0, 12) + "aA!";

					existingUser = new ApplicationUser
					{
						UserName = email,
						Email = email,
						FullName = fullName,
						EmailConfirmed = true
					};

					var createResult = await _userManager.CreateAsync(existingUser, randomPassword);

					if (!createResult.Succeeded)
						return new AuthDto
						{
							Result = Result.Failure(createResult.Errors.Select(e => e.Description)),
							Message = "Failed to create user"
						};
					 
					await _userManager.AddToRoleAsync(existingUser, "Customer");
				}
				 
				var loginInfo = new UserLoginInfo(provider, providerKey, provider);
				await _userManager.AddLoginAsync(existingUser, loginInfo);

				user = existingUser;  
			}

			// 5️⃣ Lấy roles và trả về token / user info
			var roles = await _userManager.GetRolesAsync(user);

			var authResponse = new AuthDto
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
				Roles = roles.ToList()
			}
		}
			};

			return authResponse;
		}

	}
}
