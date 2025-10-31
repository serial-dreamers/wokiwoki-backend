using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Common.Interfaces.Repositories;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class OrganizationRepository : BaseRepo<Organization, Guid>, IOrganizationRepository
	{
		public OrganizationRepository(WokiwokiDbContext context) : base(context)
		{
		}

		public async Task<Guid?> GetOrganizationIdByUserIdAsync(string userId)
		{
			var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.OwnerId == userId);
			return organization?.Id;
		}
	}
}
