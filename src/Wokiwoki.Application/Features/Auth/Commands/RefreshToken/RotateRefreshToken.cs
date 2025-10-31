using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Auth.Commands.RefreshToken
{
	public record RotateRefreshTokenCommand(
		string RefreshToken
	) : IRequest<RotateRefreshTokenDto>;

	public class RotateRefreshTokenHandler : IRequestHandler<RotateRefreshTokenCommand, RotateRefreshTokenDto>
	{ 
		private readonly IRefreshTokenService _refreshTokenService; 
		private readonly IIdentityService _identityService;
		private readonly ITokenService _tokenService;

		public RotateRefreshTokenHandler(IRefreshTokenService refreshTokenService,
			IIdentityService identityService,
			ITokenService tokenService)
		{
			_refreshTokenService = refreshTokenService;
			_identityService = identityService;
			_tokenService = tokenService;
		}
		public async Task<RotateRefreshTokenDto> Handle(RotateRefreshTokenCommand request, CancellationToken cancellationToken)
		{
			var result = await _refreshTokenService.RotateRefreshTokenAsync(request.RefreshToken);

			if (result == null)
			{
				throw new SecurityTokenException("Invalid or expired refresh token");  
			}

			var userId = result.Value.tokenEntity.UserId;
			var userName = await _identityService.GetUserNameAsync(userId);
			if (userName == null)
			{
				throw new SecurityTokenException("User not found");
			}

			var roles = await _identityService.GetRolesByUserIdAsync(userId);
			var token = _tokenService.GenerateToken(userId, userName, roles);

			return new RotateRefreshTokenDto
			{
				AccessToken = token,
				RefreshToken = result.Value.newRefreshToken
			};
		}
	}
}
