using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Common.Interfaces;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Infrastructure.Services
{
	public class IdentityService : IIdentityService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly WokiwokiDbContext _context;

		public IdentityService(UserManager<ApplicationUser> userManager, WokiwokiDbContext context)
		{
			_userManager = userManager;
			_context = context;
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

		public async Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null) 
			return Result.Failure(new[] { "User not found" });


			var identityResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

			if (!identityResult.Succeeded)
			{
				var errors = identityResult.Errors.Select(e => e.Description).ToArray();
				return Result.Failure(errors);
			}

			return Result.Success();
		}

		public async Task<string?> FindByEmailAsync(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
				return null;
			return user.Id;
		}

		public async Task<Result> ResetPasswordAsync(string userId, string newPassword)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				return Result.Failure(new[] { "User not found" });

			var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
			var identityResult = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

			if (!identityResult.Succeeded)
			{
				var errors = identityResult.Errors.Select(e => e.Description).ToArray();
				return Result.Failure(errors);
			}

			return Result.Success();
		}

		public async Task<UserDto?> GetUserByIdAsync(string userId)
		{
			var user = await _userManager.Users
				.Include(u => u.OwnedOrganization)
				.FirstOrDefaultAsync(u => u.Id == userId);

			if (user == null)
				return null;

			var roles = await _userManager.GetRolesAsync(user);

			return new UserDto
			{
				Id = user.Id,
				FullName = user.FullName ?? string.Empty,
				Email = user.Email ?? string.Empty,
				PhoneNumber = user.PhoneNumber,
				ImageUrl = user.ImageUrl,
				OwnedOrganizationId = user.OwnedOrganizationId,
				OrganizationName = user.OwnedOrganization?.Name,
				Roles = roles.ToList()
			};
		}

		public async Task<Result> UpdateUserProfileAsync(string userId, string fullName, string? phoneNumber)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				return Result.Failure(new[] { "User not found" });

			user.FullName = fullName;
			user.PhoneNumber = phoneNumber;

			var result = await _userManager.UpdateAsync(user);

			if (!result.Succeeded)
			{
				var errors = result.Errors.Select(e => e.Description).ToArray();
				return Result.Failure(errors);
			}

			return Result.Success();
		}

		public async Task<Result> UpdateUserAvatarAsync(string userId, string imageUrl)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				return Result.Failure(new[] { "User not found" });

			user.ImageUrl = imageUrl;

			var result = await _userManager.UpdateAsync(user);

			if (!result.Succeeded)
			{
				var errors = result.Errors.Select(e => e.Description).ToArray();
				return Result.Failure(errors);
			}

			return Result.Success();
		}

		public async Task<Result> UpdateUserLocationAsync(string userId, string? location, double? latitude, double? longitude)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				return Result.Failure(new[] { "User not found" });

			if (!string.IsNullOrEmpty(location))
			{
				user.Province = location;
			}

			if (latitude.HasValue && longitude.HasValue)
			{
				user.Latitude = latitude.Value;
				user.Longitude = longitude.Value;
			}

			var result = await _userManager.UpdateAsync(user);

			if (!result.Succeeded)
			{
				var errors = result.Errors.Select(e => e.Description).ToArray();
				return Result.Failure(errors);
			}

			return Result.Success();
		}

		public async Task<PaginatedList<AdminUserDto>> GetAdminUsersAsync(
			string? role,
			string? searchTerm,
			int pageNumber,
			int pageSize,
			CancellationToken cancellationToken = default)
		{
			var query = _userManager.Users.AsQueryable();

			// Apply search filter
			if (!string.IsNullOrWhiteSpace(searchTerm))
			{
				var searchLower = searchTerm.ToLower();
				query = query.Where(u =>
					(u.FullName != null && u.FullName.ToLower().Contains(searchLower)) ||
					u.Email.ToLower().Contains(searchLower));
			}

			// Get all matching users first (for role filtering)
			var allUsers = await query
				.OrderByDescending(u => u.Id)
				.ToListAsync(cancellationToken);

			// Filter by role if specified (must check roles for each user)
			if (!string.IsNullOrWhiteSpace(role))
			{
				var filteredUsers = new List<ApplicationUser>();
				foreach (var user in allUsers)
				{
					var userRoles = await _userManager.GetRolesAsync(user);
					if (userRoles.Contains(role))
					{
						filteredUsers.Add(user);
					}
				}
				allUsers = filteredUsers;
			}

			// Get total count after role filtering
			var totalCount = allUsers.Count;

			// Apply pagination
			var users = allUsers
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			var userDtos = new List<AdminUserDto>();

			foreach (var user in users)
			{
				var roles = await _userManager.GetRolesAsync(user);

				// Get booking statistics
				var totalBookings = await _context.Bookings
					.Where(b => b.UserId == user.Id)
					.CountAsync(cancellationToken);

				var totalSpent = await _context.Bookings
					.Where(b => b.UserId == user.Id)
					.SelectMany(b => b.Tickets)
					.SumAsync(t => t.Price * t.Quantity, cancellationToken);

				userDtos.Add(new AdminUserDto
				{
					Id = user.Id,
					FullName = user.FullName,
					Email = user.Email!,
					PhoneNumber = user.PhoneNumber,
					AvatarUrl = user.ImageUrl,
					Roles = roles.ToList(),
					IsEmailConfirmed = user.EmailConfirmed, 
					TotalBookings = totalBookings,
					TotalSpent = totalSpent
				});
			}

			return new PaginatedList<AdminUserDto>(
				userDtos,
				totalCount,
				pageNumber,
				pageSize
			);
		}
	}
}
