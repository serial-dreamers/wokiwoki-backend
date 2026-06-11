using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.OrganizationPayoutAccounts.Queries.GetMyPayoutAccounts
{
	public class GetMyPayoutAccountsQueryHandler : IRequestHandler<GetMyPayoutAccountsQuery, List<OrganizationPayoutAccountDto>>
	{
		private readonly IOrganizationPayoutAccountRepository _payoutAccountRepository;
		private readonly IOrganizationRepository _organizationRepository;
		private readonly IUserContext _userContext;

		public GetMyPayoutAccountsQueryHandler(
			IOrganizationPayoutAccountRepository payoutAccountRepository,
			IOrganizationRepository organizationRepository,
			IUserContext userContext)
		{
			_payoutAccountRepository = payoutAccountRepository;
			_organizationRepository = organizationRepository;
			_userContext = userContext;
		}

		public async Task<List<OrganizationPayoutAccountDto>> Handle(GetMyPayoutAccountsQuery request, CancellationToken cancellationToken)
		{
			var userId = _userContext.UserId;
			if (string.IsNullOrEmpty(userId))
				throw new UnauthorizedAccessException("User not authenticated");

			var organizationId = await _organizationRepository.GetOrganizationIdByUserIdAsync(userId);
			if (!organizationId.HasValue)
				return new List<OrganizationPayoutAccountDto>();

			var accounts = await _payoutAccountRepository.GetPayoutAccountsByOrganizationIdAsync(organizationId.Value, cancellationToken);

			return accounts.Select(a => new OrganizationPayoutAccountDto
			{
				Id = a.Id,
				OrganizationId = a.OrganizationId,
				BankCode = a.BankCode,
				BankName = a.BankName,
				AccountNumber = a.AccountNumber,
				AccountHolder = a.AccountHolder,
				LogoUrl = a.LogoUrl,
				SwiftCode = a.SwiftCode,
				Created = a.Created
			}).ToList();
		}
	}
}

