using MediatR;
using System.Data;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Auth.Commands.EmailLogin
{
	public record EmailLoginCommand( 
		string Email,
		string Password
	) : IRequest<AuthDto>;
	public class EmailLoginCommandHandler : IRequestHandler<EmailLoginCommand, AuthDto>
	{
		private readonly IIdentityService _identityService;
		private readonly IRefreshTokenService _refreshTokenService;
		private readonly ITokenService _tokenService;

		public EmailLoginCommandHandler(IIdentityService identityService, IRefreshTokenService refreshTokenService,
			ITokenService tokenService)
		{
			_identityService = identityService;
			_refreshTokenService = refreshTokenService;
			_tokenService = tokenService;
		}
		public async Task<AuthDto> Handle(EmailLoginCommand request, CancellationToken cancellationToken)
		{
			var authResponse = await _identityService.LoginAsync(request.Email, request.Password);
			if(authResponse.Result.Succeeded == true)
			{ 
				var accessToken = _tokenService.GenerateToken(authResponse.Data.User.Id, request.Email, authResponse.Data.User.Roles);

				var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(authResponse.Data.User.Id);

				authResponse.Data.AccessToken = accessToken;
				authResponse.Data.RefreshToken = refreshToken; 
			}
			return authResponse;  
		}
	}
}
