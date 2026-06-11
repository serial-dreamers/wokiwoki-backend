using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.Admin.Commands.RejectOrganization
{
	public class RejectOrganizationCommandHandler : IRequestHandler<RejectOrganizationCommand, Result>
	{
		private readonly IOrganizationRepository _organizationRepository;

		public RejectOrganizationCommandHandler(IOrganizationRepository organizationRepository)
		{
			_organizationRepository = organizationRepository;
		}

		public async Task<Result> Handle(RejectOrganizationCommand request, CancellationToken cancellationToken)
		{
			var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
			
			if (organization == null)
				return Result.Failure(new[] { "Organization not found" });

			if (string.IsNullOrWhiteSpace(request.Reason))
				return Result.Failure(new[] { "Reason is required for rejection" });

			organization.Status = OrganizationStatus.Suspended;
			// Note: Organization entity doesn't have Reason field, you might need to add it
			organization.LastModified = DateTime.UtcNow;

			await _organizationRepository.UpdateAsync(organization.Id, organization, cancellationToken);

			return Result.Success();
		}
	}
}

