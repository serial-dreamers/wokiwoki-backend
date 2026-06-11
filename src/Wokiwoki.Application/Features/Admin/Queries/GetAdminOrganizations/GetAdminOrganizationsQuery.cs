using MediatR;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Admin.Queries.GetAdminOrganizations
{
	public record GetAdminOrganizationsQuery(
		int? Status = null, // 0: Pending, 1: Accepted, 2: Suspended, null: All
		string? SearchTerm = null, // Search by name or email
		int PageNumber = 1,
		int PageSize = 20
	) : IRequest<PaginatedList<AdminOrganizationDto>>;
}

