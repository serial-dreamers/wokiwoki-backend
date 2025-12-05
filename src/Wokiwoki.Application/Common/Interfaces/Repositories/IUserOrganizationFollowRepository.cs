using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
	public interface IUserOrganizationFollowRepository : IBaseRepo<UserOrganizationFollow, Guid>
	{
		Task<bool> ExistsAsync(string userId, Guid organizationId, CancellationToken cancellationToken);

		Task<UserOrganizationFollow?> GetExistingAsync(string userId, Guid organizationId, CancellationToken cancellationToken);

		Task<List<Guid>> GetFollowedOrganizationIdsAsync(string userId, CancellationToken cancellationToken);
	}
}

