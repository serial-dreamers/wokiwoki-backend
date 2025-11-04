using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.Organizations.Commands.FollowOrganization
{
	public record FollowOrganizationCommand(Guid OrganizationId) : IRequest<Result>;

	public class FollowOrganizationCommandHandler : IRequestHandler<FollowOrganizationCommand, Result>
	{
		private readonly IUserOrganizationFollowRepository _userOrganizationFollowRepository;
		private readonly IUserContext _userContext;
		private readonly IOrganizationRepository _organizationRepository;

		public FollowOrganizationCommandHandler(
			IUserOrganizationFollowRepository userOrganizationFollowRepository,
			IUserContext userContext,
			IOrganizationRepository organizationRepository)
		{
			_userOrganizationFollowRepository = userOrganizationFollowRepository;
			_userContext = userContext;
			_organizationRepository = organizationRepository;
		}

		public async Task<Result> Handle(FollowOrganizationCommand request, CancellationToken cancellationToken)
		{
			var userId = _userContext.UserId;
			 
			var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
			if (organization == null)
				return Result.Failure(new[]{ "Organization not found" });

			// Check if user is trying to follow their own organization
			if (organization.OwnerId == userId)
				return Result.Failure(new[] { "Cannot follow your own organization" });

			// Check if already following
			var exists = await _userOrganizationFollowRepository.ExistsAsync(userId, request.OrganizationId, cancellationToken);
			if (exists)
				return Result.Failure(new[] { "Already following this organization" });

			// Create follow relationship
			var follow = new UserOrganizationFollow
			{
				UserId = userId,
				OrganizationId = request.OrganizationId
			};

			await _userOrganizationFollowRepository.CreateAsync(follow, cancellationToken);

			await _organizationRepository.IncrementFollowerCountAsync(organization.Id, cancellationToken);

			return Result.Success();
		}
	}
}

