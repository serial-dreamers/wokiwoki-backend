using MediatR;
using Microsoft.Extensions.Logging;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Users.Queries.GetUserById
{
	public record GetUserByIdQuery : IRequest<UserDto>;

	public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
	{
		private readonly IIdentityService _identityService;
		private readonly IUserContext _userContext;
		private readonly ILogger<GetUserByIdQueryHandler> _logger;

		public GetUserByIdQueryHandler(
			IIdentityService identityService,
			IUserContext userContext,
			ILogger<GetUserByIdQueryHandler> logger)
		{
			_identityService = identityService;
			_userContext = userContext;
			_logger = logger;
		}
		public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
		{
			var userId = _userContext.UserId;
			_logger.LogInformation(" [GetUserByIdQueryHandler] Current UserId: {UserId}", userId);
			var user = await _identityService.GetUserByIdAsync(userId);

			if (user == null)
				throw new Exception("User not found");

			return user;
		}
	}
}
