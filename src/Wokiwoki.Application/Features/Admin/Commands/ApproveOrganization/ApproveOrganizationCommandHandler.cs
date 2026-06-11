using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.Admin.Commands.ApproveOrganization
{
	public class ApproveOrganizationCommandHandler : IRequestHandler<ApproveOrganizationCommand, Result>
	{
		private readonly IOrganizationRepository _organizationRepository;

		public ApproveOrganizationCommandHandler(IOrganizationRepository organizationRepository)
		{
			_organizationRepository = organizationRepository;
		}

		public async Task<Result> Handle(ApproveOrganizationCommand request, CancellationToken cancellationToken)
		{
			var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
			
			if (organization == null)
				return Result.Failure(new[] { "Organization not found" });

			organization.Status = OrganizationStatus.Accepted;
			organization.LastModified = DateTime.UtcNow;

			await _organizationRepository.UpdateAsync(organization.Id, organization, cancellationToken);

			return Result.Success();
		}
	}
}

