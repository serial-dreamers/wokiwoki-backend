using MediatR; 
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Workshops.Queries.GetFilterPagedWorkshopsQuery
{
	public record GetWorkshopsByOrganizationIdQuery(
		Guid organizationId,
		int PageNumber = 1,
		int PageSize = 10
		) : IRequest<PaginatedList<WorkshopDto>>;

	public class GetWorkshopsByOrganizationId
	{
	}
}
