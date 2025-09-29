using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
	public interface IRefreshTokenRepository : IBaseRepo<RefreshToken, Guid>
	{
		Task AddRefreshTokenAsync(RefreshToken refreshToken);
		Task<RefreshToken?> GetRefreshTokenAsync(string token);
	}
}
