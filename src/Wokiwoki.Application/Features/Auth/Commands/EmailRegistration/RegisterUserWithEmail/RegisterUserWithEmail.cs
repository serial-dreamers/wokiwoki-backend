using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Auth.Commands.EmailRegistration.RegisterUserWithEmail
{
	public record RegisterUserWithEmailCommand(
		string Email, 
		string Password,
		string FullName,
		string VerificationCode
	) : IRequest<AuthDto>;

	public class RegisterUserWithEmailCommandHandler
		: IRequestHandler<RegisterUserWithEmailCommand, AuthDto>
	{
		private readonly IIdentityService _identityService;
		private readonly IRefreshTokenService _refreshTokenService;
		private readonly ITokenService _tokenService;
		private readonly IRedisCacheService _redisCacheService;


		public RegisterUserWithEmailCommandHandler(IIdentityService identityService, IRefreshTokenService refreshTokenService, ITokenService tokenService, IRedisCacheService redisCacheService)
		{
			_identityService = identityService;
			_refreshTokenService = refreshTokenService;
			_tokenService = tokenService;
			_redisCacheService = redisCacheService;
		}

		public async Task<AuthDto> Handle(RegisterUserWithEmailCommand request, CancellationToken cancellationToken)
		{
			var cachedCode = await _redisCacheService.GetAsync($"verify:{request.Email}");

			if (cachedCode != request.VerificationCode)
				throw new Exception("Email not verified or code expired");

			await _redisCacheService.RemoveAsync($"verify:{request.Email}");

			var (result, userId) = await _identityService.CreateUserAsync(
				request.Email, 
				request.Password,
				request.FullName
			); 

			var roles = await _identityService.GetRolesByUserIdAsync(userId); 

			var accessToken = _tokenService.GenerateToken(userId, request.Email, roles);
			var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(userId);

			var data = new Data
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken,
				User =
				{
					Id = userId,
					Name = request.FullName,
					Email = request.Email,
					Roles = roles.ToList()
				}
			};

			var auth = new AuthDto
			{
				Result = result,
				Message = "Register Successs",
				Data = data 
			};

			return auth; 
		}
	}
}
