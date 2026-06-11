using MediatR;

namespace Wokiwoki.Application.Features.OrganizationPayoutAccounts.Commands.CreatePayoutAccount
{
	public record CreatePayoutAccountCommand(
		string BankCode,
		string BankName,
		string AccountNumber,
		string AccountHolder,
		string? LogoUrl,
		string SwiftCode
	) : IRequest<Guid>;
}

