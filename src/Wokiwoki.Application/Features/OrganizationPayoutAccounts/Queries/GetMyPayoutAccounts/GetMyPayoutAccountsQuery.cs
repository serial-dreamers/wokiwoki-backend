using MediatR;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.OrganizationPayoutAccounts.Queries.GetMyPayoutAccounts
{
	public record GetMyPayoutAccountsQuery : IRequest<List<OrganizationPayoutAccountDto>>;
}

