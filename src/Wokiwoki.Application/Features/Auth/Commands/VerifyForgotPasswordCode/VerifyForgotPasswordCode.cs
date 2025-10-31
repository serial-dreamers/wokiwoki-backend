using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.Auth.Commands.VerifyForgotPasswordCode
{
	public record VerifyForgotPasswordCodeCommand(string Email, string Code) : IRequest<Result>;

	public class VerifyForgotPasswordCodeCommandHandler : IRequestHandler<VerifyForgotPasswordCodeCommand, Result>
	{
		private readonly IRedisCacheService _redisCacheService;
		public VerifyForgotPasswordCodeCommandHandler(IRedisCacheService redisCacheService)
		{
			_redisCacheService = redisCacheService;
		}
		public async Task<Result> Handle(VerifyForgotPasswordCodeCommand request, CancellationToken cancellationToken)
		{
			var normalizedEmail = request.Email.Trim().ToLowerInvariant();
			var cached = await _redisCacheService.GetAsync($"forgot:{normalizedEmail}");

			if (cached == null || cached != request.Code)
				return Result.Failure(new[] { "Invalid or expired code" });

			 
			await _redisCacheService.SetAsync($"forgot:verified:{normalizedEmail}", "true", TimeSpan.FromMinutes(10));

			return Result.Success();
		}
	}
}
