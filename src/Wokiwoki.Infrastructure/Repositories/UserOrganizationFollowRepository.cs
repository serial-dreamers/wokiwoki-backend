using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class UserOrganizationFollowRepository : BaseRepo<UserOrganizationFollow, Guid>, IUserOrganizationFollowRepository
	{
		public UserOrganizationFollowRepository(WokiwokiDbContext context) : base(context)
		{
		}

		public async Task<bool> ExistsAsync(string userId, Guid organizationId, CancellationToken cancellationToken)
		{
			return await _context.UserOrganizationFollows
				.AnyAsync(x => x.UserId == userId && x.OrganizationId == organizationId, cancellationToken);
		}

		public async Task<UserOrganizationFollow?> GetExistingAsync(string userId, Guid organizationId, CancellationToken cancellationToken)
		{
			return await _context.UserOrganizationFollows
				.FirstOrDefaultAsync(x => x.UserId == userId && x.OrganizationId == organizationId, cancellationToken);
		}

		public async Task<List<Guid>> GetFollowedOrganizationIdsAsync(string userId, CancellationToken cancellationToken)
		{
			return await _context.UserOrganizationFollows
				.Where(x => x.UserId == userId)
				.Select(x => x.OrganizationId)
				.ToListAsync(cancellationToken);
		}
	}
}

