using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.Organizations.Commands.UpdateOrganizationInfo
{
	public class UpdateOrganizationInfoCommandHandler : IRequestHandler<UpdateOrganizationInfoCommand, Result>
	{
		private readonly IOrganizationRepository _organizationRepository;
		private readonly IUserContext _userContext;

		public UpdateOrganizationInfoCommandHandler(
			IOrganizationRepository organizationRepository,
			IUserContext userContext)
		{
			_organizationRepository = organizationRepository;
			_userContext = userContext;
		}

		public async Task<Result> Handle(UpdateOrganizationInfoCommand request, CancellationToken cancellationToken)
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

			// Update fields
			organization.Name = request.Name;
			organization.Description = request.Description;
			organization.ContactEmail = request.ContactEmail;
			organization.ContactPhone = request.ContactPhone;
			organization.Street = request.Street;
			organization.Commune = request.Commune;
			organization.Province = request.Province;

			var success = await _organizationRepository.UpdateAsync(organizationId.Value, organization, cancellationToken);
			
			if (!success)
				return Result.Failure(new[] { "Failed to update organization" });

			return Result.Success();
		}
	}
}

