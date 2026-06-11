using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Admin.Queries.GetAdminUsers
{
	public class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, PaginatedList<AdminUserDto>>
	{
		private readonly IIdentityService _identityService;

		public GetAdminUsersQueryHandler(IIdentityService identityService)
		{
			_identityService = identityService;
		}

		public async Task<PaginatedList<AdminUserDto>> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
		{
			return await _identityService.GetAdminUsersAsync(
				request.Role,
				request.SearchTerm,
				request.PageNumber,
				request.PageSize,
				cancellationToken);
		}
	}
}

