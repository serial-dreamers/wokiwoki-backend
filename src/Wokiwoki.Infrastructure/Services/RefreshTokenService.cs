using System.Security.Cryptography;
using System.Text;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Utils;

namespace Wokiwoki.Infrastructure.Services
{
	public class RefreshTokenService : IRefreshTokenService
	{
		private readonly IRefreshTokenRepository _refreshTokenRepository;
		private readonly IUuidService _uuidService;

		public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository,
			IUuidService uuidService)
		{
			_refreshTokenRepository = refreshTokenRepository;
			_uuidService = uuidService;
		}

		public async Task<string> GenerateRefreshTokenAsync(string userId)
		{
			var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
			var tokenHash = ComputeSha256Hash(rawToken);


			var token = new RefreshToken
			{
				Id = _uuidService.NewGuid(),
				UserId = userId,
				Token = tokenHash,
				Created = DateTime.UtcNow,
				ExpiresAt = DateTime.UtcNow.AddDays(7),
				Revoked = false,
				CreatedBy = userId
			};

			await _refreshTokenRepository.CreateAsync(token);
			return rawToken;
		}

		public async Task<RefreshToken?> GetRefreshTokenAsync(string rawToken)
		{
			var tokenHash = ComputeSha256Hash(rawToken);
			return await _refreshTokenRepository.GetRefreshTokenByHashAsync(tokenHash);
		}

		public async Task<(string newRefreshToken, RefreshToken tokenEntity)?> RotateRefreshTokenAsync(string rawToken)
		{
			var tokenHash = ComputeSha256Hash(rawToken);
			var existingToken = await _refreshTokenRepository.GetRefreshTokenByHashAsync(tokenHash);
			if (existingToken == null) return null;
			 
			if (existingToken.ExpiresAt <= DateTime.UtcNow || existingToken.Revoked)
				return null;
			 
			existingToken.Revoked = true;
			await _refreshTokenRepository.UpdateAsync(existingToken.Id, existingToken);
			 
			var newRawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
			var newHash = ComputeSha256Hash(newRawToken);

			var newTokenEntity = new RefreshToken
			{
				Id = _uuidService.NewGuid(),
				UserId = existingToken.UserId,
				Token = newHash,
				Created = DateTime.UtcNow,
				ExpiresAt = DateTime.UtcNow.AddDays(7),
				Revoked = false,
				CreatedBy = existingToken.UserId
			};
	
			await _refreshTokenRepository.CreateAsync(newTokenEntity);

			return (newRawToken, newTokenEntity);
		}


		private static string ComputeSha256Hash(string raw)
		{
			using var sha = SHA256.Create();
			var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
			return Convert.ToHexString(bytes);  
		}

		public async Task RevokeAllUserTokensAsync(string userId)
		{
			var userTokens = await _refreshTokenRepository.GetTokensByUserIdAsync(userId);

			foreach (var token in userTokens)
			{
				if (!token.Revoked)
				{
					token.Revoked = true;
					await _refreshTokenRepository.UpdateAsync(token.Id, token);
				}
			}
		}
	}
}
