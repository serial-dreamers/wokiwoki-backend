using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.OrganizationPayoutAccounts.Commands.UpdatePayoutAccount
{
	public class UpdatePayoutAccountCommandHandler : IRequestHandler<UpdatePayoutAccountCommand, Result>
	{
		private readonly IOrganizationPayoutAccountRepository _payoutAccountRepository;
		private readonly IOrganizationRepository _organizationRepository;
		private readonly IUserContext _userContext;

		public UpdatePayoutAccountCommandHandler(
			IOrganizationPayoutAccountRepository payoutAccountRepository,
			IOrganizationRepository organizationRepository,
			IUserContext userContext)
		{
			_payoutAccountRepository = payoutAccountRepository;
			_organizationRepository = organizationRepository;
			_userContext = userContext;
		}

		public async Task<Result> Handle(UpdatePayoutAccountCommand request, CancellationToken cancellationToken)
		{
			var userId = _userContext.UserId;
			if (string.IsNullOrEmpty(userId))
				return Result.Failure(new[] { "User not authenticated" });

			var organizationId = await _organizationRepository.GetOrganizationIdByUserIdAsync(userId);
			if (!organizationId.HasValue)
				return Result.Failure(new[] { "Organization not found for this user" });

			var payoutAccount = await _payoutAccountRepository.GetByIdAsync(request.Id, cancellationToken);
			if (payoutAccount == null)
				return Result.Failure(new[] { "Payout account not found" });

			if (payoutAccount.OrganizationId != organizationId.Value)
				return Result.Failure(new[] { "Unauthorized to update this payout account" });

			payoutAccount.BankCode = request.BankCode;
			payoutAccount.BankName = request.BankName;
			payoutAccount.AccountNumber = request.AccountNumber;
			payoutAccount.AccountHolder = request.AccountHolder;
			payoutAccount.LogoUrl = request.LogoUrl;
			payoutAccount.SwiftCode = request.SwiftCode;

			var success = await _payoutAccountRepository.UpdateAsync(request.Id, payoutAccount, cancellationToken);
			
			if (!success)
				return Result.Failure(new[] { "Failed to update payout account" });

			return Result.Success();
		}
	}
}

