using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Services
{
	public interface IRefreshTokenService
	{
		Task<string> GenerateRefreshTokenAsync(string userId);
		Task<RefreshToken?> GetRefreshTokenAsync(string rawToken);
		Task<(string newRefreshToken, RefreshToken tokenEntity)?> RotateRefreshTokenAsync(string rawToken);

		Task RevokeAllUserTokensAsync(string userId);
	}
}
