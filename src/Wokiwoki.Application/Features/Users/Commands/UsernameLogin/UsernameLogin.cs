using MediatR;
using System.Data;
using Wokiwoki.Application.Common.Interfaces.Services;

namespace Wokiwoki.Application.Features.Users.Commands.UsernameLogin
{
	public record UsernameLoginCommand( 
		string Username,
		string Password
	) : IRequest<AuthResponseDto>;
	public class UsernameLoginCommandHandler : IRequestHandler<UsernameLoginCommand, AuthResponseDto>
	{
		private readonly IIdentityService _identityService;
		private readonly IRefreshTokenService _refreshTokenService;
		private readonly ITokenService _tokenService;

		public UsernameLoginCommandHandler(IIdentityService identityService, IRefreshTokenService refreshTokenService,
			ITokenService tokenService)
		{
			_identityService = identityService;
			_refreshTokenService = refreshTokenService;
			_tokenService = tokenService;
		}
		public async Task<AuthResponseDto> Handle(UsernameLoginCommand request, CancellationToken cancellationToken)
		{
			var authResponse = await _identityService.LoginAsync(request.Username, request.Password);
			if(authResponse.Result.Succeeded == true)
			{ 
				var accessToken = _tokenService.GenerateToken(authResponse.Data.User.Id, request.Username, authResponse.Data.User.Role);
				var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(authResponse.Data.User.Id);

				authResponse.Data.AccessToken = accessToken;
				authResponse.Data.RefreshToken = refreshToken; 
			}
			return authResponse;  
		}
	}
}
