using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.OrganizationPayoutAccounts.Commands.CreatePayoutAccount
{
	public class CreatePayoutAccountCommandHandler : IRequestHandler<CreatePayoutAccountCommand, Guid>
	{
		private readonly IOrganizationPayoutAccountRepository _payoutAccountRepository;
		private readonly IOrganizationRepository _organizationRepository;
		private readonly IUuidService _uuidService;
		private readonly IUserContext _userContext;

		public CreatePayoutAccountCommandHandler(
			IOrganizationPayoutAccountRepository payoutAccountRepository,
			IOrganizationRepository organizationRepository,
			IUuidService uuidService,
			IUserContext userContext)
		{
			_payoutAccountRepository = payoutAccountRepository;
			_organizationRepository = organizationRepository;
			_uuidService = uuidService;
			_userContext = userContext;
		}

		public async Task<Guid> Handle(CreatePayoutAccountCommand request, CancellationToken cancellationToken)
		{
			var userId = _userContext.UserId;
			if (string.IsNullOrEmpty(userId))
				throw new UnauthorizedAccessException("User not authenticated");

			var organizationId = await _organizationRepository.GetOrganizationIdByUserIdAsync(userId);
			if (!organizationId.HasValue)
				throw new Exception("Organization not found for this user");

			var payoutAccount = new OrganizationPayoutAccount
			{
				Id = _uuidService.NewGuid(),
				OrganizationId = organizationId.Value,
				BankCode = request.BankCode,
				BankName = request.BankName,
				AccountNumber = request.AccountNumber,
				AccountHolder = request.AccountHolder,
				LogoUrl = request.LogoUrl,
				SwiftCode = request.SwiftCode, 
				Created = DateTime.UtcNow,
				CreatedBy = userId
			};

			await _payoutAccountRepository.CreateAsync(payoutAccount, cancellationToken);

			return payoutAccount.Id;
		}
	}
}

