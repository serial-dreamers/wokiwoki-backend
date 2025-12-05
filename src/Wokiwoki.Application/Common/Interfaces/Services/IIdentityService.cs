//using Wokiwoki.Application.Auth.Users.Commands;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Common.Interfaces.Services
{
	public interface IIdentityService
	{
		Task<AuthDto> LoginAsync(string email, string password);

		Task<string?> GetUserNameAsync(string userId);

		Task<(Result Result, string UserId)> CreateUserAsync(string email, string password, string fullname);

		Task<IEnumerable<string>> GetRolesByUserIdAsync(string userId); 

		Task<AuthDto?> LoginGoogleConfirm(string providerKey, string password, string fullname);

		Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

		Task<string?> FindByEmailAsync(string email);

		Task<Result> ResetPasswordAsync(string userId, string newPassword);

		Task<UserDto?> GetUserByIdAsync(string userId);

		Task<Result> UpdateUserProfileAsync(string userId, string fullName, string? phoneNumber);

		Task<Result> UpdateUserAvatarAsync(string userId, string imageUrl);

		Task<Result> UpdateUserLocationAsync(string userId, string? location, double? latitude, double? longitude);
	}
}
