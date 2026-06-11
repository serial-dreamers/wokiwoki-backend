using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Infrastructure.Data;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class OrganizationPayoutAccountRepository : BaseRepo<OrganizationPayoutAccount, Guid>, IOrganizationPayoutAccountRepository
	{
		public OrganizationPayoutAccountRepository(WokiwokiDbContext context) : base(context)
		{
		}

		public async Task<List<OrganizationPayoutAccount>> GetPayoutAccountsByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default)
		{
			return await _context.OrganizationPayoutAccounts
				.Where(a => a.OrganizationId == organizationId)
				.OrderByDescending(a => a.Id)
				.ToListAsync(cancellationToken);
		}
	}
}
