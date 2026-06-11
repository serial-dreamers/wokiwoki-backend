using MediatR;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.OrganizationPayoutAccounts.Commands.DeletePayoutAccount
{
	public record DeletePayoutAccountCommand(Guid Id) : IRequest<Result>;
}

