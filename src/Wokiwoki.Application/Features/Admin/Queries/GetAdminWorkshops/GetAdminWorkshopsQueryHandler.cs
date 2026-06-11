using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Admin.Queries.GetAdminWorkshops
{
	public class GetAdminWorkshopsQueryHandler : IRequestHandler<GetAdminWorkshopsQuery, PaginatedList<AdminWorkshopDto>>
	{
		private readonly IWorkshopRepository _workshopRepository;

		public GetAdminWorkshopsQueryHandler(IWorkshopRepository workshopRepository)
		{
			_workshopRepository = workshopRepository;
		}

		public async Task<PaginatedList<AdminWorkshopDto>> Handle(GetAdminWorkshopsQuery request, CancellationToken cancellationToken)
		{
			return await _workshopRepository.GetAdminWorkshopsAsync(
				request.Status,
				request.SearchTerm,
				request.OrganizationId,
				request.PageNumber,
				request.PageSize,
				cancellationToken);
		}
	}
}

