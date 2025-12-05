using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.Users.Commands.UpdateUserProfile
{
	public record UpdateUserProfileCommand : IRequest<Result>
	{
		public string FullName { get; set; } = string.Empty;
		public string? PhoneNumber { get; set; }
	}

	public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Result>
	{
		private readonly IIdentityService _identityService;
		private readonly IUserContext _userContext;

		public UpdateUserProfileCommandHandler(IIdentityService identityService, IUserContext userContext)
		{
			_identityService = identityService;
			_userContext = userContext;
		}

		public async Task<Result> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
		{
			var userId = _userContext.UserId;

			var result = await _identityService.UpdateUserProfileAsync(userId, request.FullName, request.PhoneNumber);

			return result;
		}
	}
}

