using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.Organizations.Commands.UpdateOrganizationLogo
{
	public class UpdateOrganizationLogoCommandHandler : IRequestHandler<UpdateOrganizationLogoCommand, Result>
	{
		private readonly IOrganizationRepository _organizationRepository;
		private readonly IUserContext _userContext;

		public UpdateOrganizationLogoCommandHandler(
			IOrganizationRepository organizationRepository,
			IUserContext userContext)
		{
			_organizationRepository = organizationRepository;
			_userContext = userContext;
		}

		public async Task<Result> Handle(UpdateOrganizationLogoCommand request, CancellationToken cancellationToken)
		{
			var userId = _userContext.UserId;
			if (string.IsNullOrEmpty(userId))
				return Result.Failure(new[] { "User not authenticated" });

			var organizationId = await _organizationRepository.GetOrganizationIdByUserIdAsync(userId);
			if (!organizationId.HasValue)
				return Result.Failure(new[] { "Organization not found for this user" });

			var organization = await _organizationRepository.GetByIdAsync(organizationId.Value, cancellationToken);
			if (organization == null)
				return Result.Failure(new[] { "Organization not found" });

			organization.LogoUrl = request.LogoUrl;

			var success = await _organizationRepository.UpdateAsync(organizationId.Value, organization, cancellationToken);
			
			if (!success)
				return Result.Failure(new[] { "Failed to update organization logo" });

			return Result.Success();
		}
	}
}

