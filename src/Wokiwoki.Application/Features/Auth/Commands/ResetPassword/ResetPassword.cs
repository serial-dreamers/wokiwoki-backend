using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Auth.Commands.ResetPassword
{
	public record ResetPasswordCommand(
		string Email, string NewPassword, string ConfirmNewPassword
	) : IRequest<Result>;

	public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
	{
		private readonly IRedisCacheService _redisCacheService;
		private readonly IIdentityService _identityService;
		private readonly IRefreshTokenService _refreshTokenService;

		public ResetPasswordCommandHandler(
			IRedisCacheService redisCacheService,
			IIdentityService identityService,
			IRefreshTokenService refreshTokenService)
		{
			_redisCacheService = redisCacheService;
			_identityService = identityService;
			_refreshTokenService = refreshTokenService;
		}

		public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
		{
			if (request.NewPassword != request.ConfirmNewPassword)
				return Result.Failure(new[] { "Passwords do not match" });

			var normalizedEmail = request.Email.Trim().ToLowerInvariant();

			var verified = await _redisCacheService.GetAsync($"forgot:verified:{normalizedEmail}");
			if (verified == null)
				return Result.Failure(new[] { "Email not verified for password reset" });

			var userId = await _identityService.FindByEmailAsync(request.Email);
			if (userId == null)
				return Result.Failure(new[] { "User not found" });

			var result = await _identityService.ResetPasswordAsync(userId, request.NewPassword);

			if (!result.Succeeded)
				return Result.Failure(result.Errors);

			await _refreshTokenService.RevokeAllUserTokensAsync(userId); 

			return Result.Success();
		}
	}
}
