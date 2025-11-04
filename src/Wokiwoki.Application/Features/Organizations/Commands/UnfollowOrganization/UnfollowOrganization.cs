using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.Organizations.Commands.UnfollowOrganization
{
	public record UnfollowOrganizationCommand(Guid OrganizationId) : IRequest<Result>;

	public class UnfollowOrganizationCommandHandler : IRequestHandler<UnfollowOrganizationCommand, Result>
	{
		private readonly IUserOrganizationFollowRepository _userOrganizationFollowRepository;
		private readonly IUserContext _userContext;
		private readonly IOrganizationRepository _organizationRepository;

		public UnfollowOrganizationCommandHandler(
			IUserOrganizationFollowRepository userOrganizationFollowRepository,
			IUserContext userContext,
			IOrganizationRepository organizationRepository)
		{
			_userOrganizationFollowRepository = userOrganizationFollowRepository;
			_userContext = userContext;
			_organizationRepository = organizationRepository;
		}

		public async Task<Result> Handle(UnfollowOrganizationCommand request, CancellationToken cancellationToken)
		{
			var userId = _userContext.UserId;

			var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
			if (organization == null)
				return Result.Failure(new[] { "Organization not found" });
			 
			var follow = await _userOrganizationFollowRepository.GetExistingAsync(userId, request.OrganizationId, cancellationToken);
			if (follow == null)
				return Result.Failure(new[] { "Not following this organization" });

			 
			await _userOrganizationFollowRepository.DeleteAsync(follow.Id, cancellationToken);

			await _organizationRepository.DecrementFollowerCountAsync(follow.Id, cancellationToken);

			return Result.Success();
		}
	}
}

