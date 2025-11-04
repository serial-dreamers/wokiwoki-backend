using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

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

		public async Task<List<Organization>> GetOrganizationsByIdsAsync(List<Guid> organizationIds, CancellationToken cancellationToken = default)
		{
			return await _context.Organizations
				.Where(org => organizationIds.Contains(org.Id)
					&& org.IsActive
					&& org.Status == OrganizationStatus.Accepted)
				.ToListAsync(cancellationToken);
		}

		public async Task IncrementFollowerCountAsync(Guid organizationId, CancellationToken cancellationToken)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync(
				$"UPDATE organization SET followercount = followercount + 1 WHERE id = {organizationId}",
				cancellationToken
			);
		}


		public async Task DecrementFollowerCountAsync(Guid organizationId, CancellationToken cancellationToken)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync(
		$"UPDATE organization SET followercount = CASE WHEN followercount > 0 THEN followercount - 1 ELSE 0 END WHERE id = {organizationId}",
			cancellationToken
			);
		}
	}
}
