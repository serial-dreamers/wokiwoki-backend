using MediatR;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.OrganizationPayoutAccounts.Commands.UpdatePayoutAccount
{
	public record UpdatePayoutAccountCommand(
		Guid Id,
		string BankCode,
		string BankName,
		string AccountNumber,
		string AccountHolder,
		string? LogoUrl,
		string SwiftCode
	) : IRequest<Result>;
}

