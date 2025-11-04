using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
	public interface IUserTagPreferenceRepository : IBaseRepo<UserTagPreference, Guid>
	{
		Task<List<UserTagPreference>> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
	}
}

