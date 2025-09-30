using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;

namespace Wokiwoki.Application.Features.Users.Commands.EmailRegistration.RegisterUserWithEmail
{
	public record RegisterUserWithEmailCommand(
		string Email,
		string Username,
		string Password,
		string FullName,
		string VerificationCode
	) : IRequest<AuthResponseDto>;

	public class RegisterUserWithEmailCommandHandler
		: IRequestHandler<RegisterUserWithEmailCommand, AuthResponseDto>
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

		public async Task<AuthResponseDto> Handle(RegisterUserWithEmailCommand request, CancellationToken cancellationToken)
		{
			var cachedCode = await _redisCacheService.GetAsync($"verify:{request.Email}");

			if (cachedCode != request.VerificationCode)
				throw new Exception("Email not verified or code expired");

			await _redisCacheService.RemoveAsync($"verify:{request.Email}");

			var (result, userId) = await _identityService.CreateUserAsync(
				request.Email,
				request.Username,
				request.Password,
				request.FullName
			); 

			var roles = await _identityService.GetRolesByUserIdAsync(userId);
			var role = roles.FirstOrDefault() ?? "Customer";
			var accessToken = _tokenService.GenerateToken(userId, request.Username, role);
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
					Role = role
				}
			};

			var auth = new AuthResponseDto
			{
				Result = result,
				Message = "Register Successs",
				Data = data 
			};

			return auth; 
		}
	}
}
