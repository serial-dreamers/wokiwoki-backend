using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.Users.Commands.EmailRegistration.VerifyEmailCode
{
	public record VerifyEmailCodeCommand(string Email, string Code) : IRequest<Result>;

	public class VerifyEmailCodeCommandHandle : IRequestHandler<VerifyEmailCodeCommand, Result>
	{
		private readonly IRedisCacheService _redisCacheService;

		public VerifyEmailCodeCommandHandle(IRedisCacheService redisCacheService)
		{
			_redisCacheService = redisCacheService;
		}

		public async Task<Result> Handle(VerifyEmailCodeCommand request, CancellationToken cancellationToken)
		{
			if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Code))
				return Result.Failure(new[] { "Invalid request" });

			var cached = await _redisCacheService.GetAsync(request.Email);

			if (cached == null || cached != request.Code)
				return Result.Failure(new[] { "Invalid or expired code" });
			await _redisCacheService.RemoveAsync(request.Email);

			return Result.Success();
		}
	}
}
