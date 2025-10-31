using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.Auth.Commands.EmailRegistration.VerifyEmailCode
{
	public record VerifyEmailCodeCommand(string Email, string Code) : IRequest<Result>;

	public class VerifyEmailCodeCommandHandler : IRequestHandler<VerifyEmailCodeCommand, Result>
	{
		private readonly IRedisCacheService _redisCacheService;

		public VerifyEmailCodeCommandHandler(IRedisCacheService redisCacheService)
		{
			_redisCacheService = redisCacheService;
		}

		public async Task<Result> Handle(VerifyEmailCodeCommand request, CancellationToken cancellationToken)
		{
			if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Code))
				return Result.Failure(new[] { "Invalid request" });

			var normalizedEmail = request.Email.Trim().ToLowerInvariant();
			var cached = await _redisCacheService.GetAsync($"verify:{normalizedEmail}");

			if (cached == null || cached != request.Code)
				return Result.Failure(new[] { "Invalid or expired code" });
			//await _redisCacheService.RemoveAsync(normalizedEmail);

			return Result.Success();
		}
	}
}
