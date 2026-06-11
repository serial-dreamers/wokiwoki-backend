using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.OrganizationPayoutAccounts.Commands.DeletePayoutAccount
{
	public class DeletePayoutAccountCommandHandler : IRequestHandler<DeletePayoutAccountCommand, Result>
	{
		private readonly IOrganizationPayoutAccountRepository _payoutAccountRepository;
		private readonly IOrganizationRepository _organizationRepository;
		private readonly IUserContext _userContext;

		public DeletePayoutAccountCommandHandler(
			IOrganizationPayoutAccountRepository payoutAccountRepository,
			IOrganizationRepository organizationRepository,
			IUserContext userContext)
		{
			_payoutAccountRepository = payoutAccountRepository;
			_organizationRepository = organizationRepository;
			_userContext = userContext;
		}

		public async Task<Result> Handle(DeletePayoutAccountCommand request, CancellationToken cancellationToken)
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
				return Result.Failure(new[] { "Unauthorized to delete this payout account" });

			var success = await _payoutAccountRepository.DeleteAsync(request.Id, cancellationToken);
			
			if (!success)
				return Result.Failure(new[] { "Failed to delete payout account" });

			return Result.Success();
		}
	}
}

