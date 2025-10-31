using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.Auth.Commands.ChangePassword
{
	public record ChangePasswordCommand(
		string UserId,
		string CurrentPassword,
		string NewPassword,
		string ConfirmNewPassword
	) : IRequest<Result>;

	public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
	{
		private readonly IIdentityService _identityService;
		private readonly IRefreshTokenService _refreshTokenService;

		public ChangePasswordCommandHandler(IIdentityService identityService, 
			IRefreshTokenService refreshTokenService)
		{
			_identityService = identityService;
			_refreshTokenService = refreshTokenService;
		}
		public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
		{
			if (request.NewPassword != request.ConfirmNewPassword)
				return Result.Failure(new[] {"Passwords do not match"});

			var result = await _identityService.ChangePasswordAsync(request.UserId, request.CurrentPassword, request.NewPassword);

			if (!result.Succeeded)
				return Result.Failure(result.Errors);

			await _refreshTokenService.RevokeAllUserTokensAsync(request.UserId);
			return Result.Success();
		}
	}
}
