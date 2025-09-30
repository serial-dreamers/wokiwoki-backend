using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.Features.Users.Commands;

namespace Wokiwoki.Application.Common.Interfaces.Services
{
	public interface IIdentityService
	{
		Task<AuthResponseDto> LoginAsync(string username, string password);

		Task<string?> GetUserNameAsync(string userId);

		Task<(Result Result, string UserId)> CreateUserAsync(string email, string username, string password, string fullname);

		Task<IList<string>> GetRolesByUserIdAsync(string userId);
	}
}
