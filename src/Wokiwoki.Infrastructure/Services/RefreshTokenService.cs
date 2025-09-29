using System.Security.Cryptography;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;

namespace Wokiwoki.Infrastructure.Services
{
	public class RefreshTokenService : IRefreshTokenService
	{
		private readonly IRefreshTokenRepository _refreshTokenRepository;

		public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
		{
			_refreshTokenRepository = refreshTokenRepository;
		}

		public async Task<string> GenerateRefreshTokenAsync(string userId)
		{
			var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
			var token = new RefreshToken
			{
				UserId = userId,
				Token = refreshToken,
				Created = DateTime.UtcNow,
				ExpiresAt = DateTime.UtcNow.AddDays(7),
				Revoked = false
			};

			await _refreshTokenRepository.CreateAsync(token);
			return refreshToken;
		}

		public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
		{
			return await _refreshTokenRepository.GetRefreshTokenAsync(token);
		}
	}
}
