using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Admin.Queries.GetAdminOrganizations
{
	public class GetAdminOrganizationsQueryHandler : IRequestHandler<GetAdminOrganizationsQuery, PaginatedList<AdminOrganizationDto>>
	{
		private readonly IOrganizationRepository _organizationRepository;

		public GetAdminOrganizationsQueryHandler(IOrganizationRepository organizationRepository)
		{
			_organizationRepository = organizationRepository;
		}

		public async Task<PaginatedList<AdminOrganizationDto>> Handle(GetAdminOrganizationsQuery request, CancellationToken cancellationToken)
		{
			return await _organizationRepository.GetAdminOrganizationsAsync(
				request.Status,
				request.SearchTerm,
				request.PageNumber,
				request.PageSize,
				cancellationToken);
		}
	}
}

