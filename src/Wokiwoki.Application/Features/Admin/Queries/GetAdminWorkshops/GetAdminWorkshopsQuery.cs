using MediatR;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Admin.Queries.GetAdminWorkshops
{
	public record GetAdminWorkshopsQuery(
		int? Status = null, // 0: Draft, 1: PendingReview, 2: Published, 3: Hidden, 4: Cancelled, null: All
		string? SearchTerm = null, // Search by title or organization name
		Guid? OrganizationId = null,
		int PageNumber = 1,
		int PageSize = 20
	) : IRequest<PaginatedList<AdminWorkshopDto>>;
}

