using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Users.Commands.GoogleLogin
{
	public record GoogleLoginCommand(string tokenId) : IRequest<AuthDto?>;

	public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, AuthDto?>
	{ 
		private readonly IGoogleService _googleService;
		private readonly IIdentityService _identityService;
		private readonly ITokenService _tokenService;
		private readonly IRefreshTokenService _refreshTokenService;


		public GoogleLoginCommandHandler(IGoogleService googleService, IIdentityService identityService, ITokenService tokenService, IRefreshTokenService refreshTokenService	 )
		{
			_googleService = googleService;
			_identityService = identityService;
			_tokenService = tokenService;
			_refreshTokenService = refreshTokenService;
		}

		public async Task<AuthDto?> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
		{
			var googleUser = await _googleService.GetUserByTokenId(request.tokenId);

			if(googleUser == null)
				 return new AuthDto { Result = Result.Failure(["Invalid Google token"]) };

			var user = await _identityService.LoginGoogleConfirm(googleUser.ProviderKey, googleUser.Email, googleUser.FullName);

			if(user == null) return new AuthDto { Result = Result.Failure(["User creation or login failed"]) };

			var accessToken = _tokenService.GenerateToken(user.Data.User.Id, user.Data.User.Email, user.Data.User.Roles);
			var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Data.User.Id);

			user.Data.AccessToken = accessToken;
			user.Data.RefreshToken = refreshToken;

			return user;
		}
	}
}
