using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class UserTagPreferenceRepository : BaseRepo<UserTagPreference, Guid>, IUserTagPreferenceRepository
	{
		public UserTagPreferenceRepository(WokiwokiDbContext context) : base(context)
		{
		}

		public async Task<List<UserTagPreference>> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
		{
			return await _context.UserTagPreferences
				.Where(x => x.UserId == userId)
				.ToListAsync(cancellationToken);
		}
	}
}

