using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Services
{
	public interface IRefreshTokenService
	{
		Task<string> GenerateRefreshTokenAsync(string userId);
		Task<RefreshToken?> GetRefreshTokenAsync(string token);
	}
}
