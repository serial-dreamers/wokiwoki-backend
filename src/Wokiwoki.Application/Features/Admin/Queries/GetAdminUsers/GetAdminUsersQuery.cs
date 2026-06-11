using MediatR;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Admin.Queries.GetAdminUsers
{
	public record GetAdminUsersQuery(
		string? Role = null, // Filter by role: Customer, Organizer, Admin
		string? SearchTerm = null, // Search by name or email
		int PageNumber = 1,
		int PageSize = 20
	) : IRequest<PaginatedList<AdminUserDto>>;
}

