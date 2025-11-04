using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
	public interface IOrganizationRepository : IBaseRepo<Organization, Guid>
	{
		Task<Guid?> GetOrganizationIdByUserIdAsync(string userId);

		Task<List<Organization>> GetOrganizationsByIdsAsync(List<Guid> organizationIds, CancellationToken cancellationToken = default);

		Task IncrementFollowerCountAsync(Guid organizationId, CancellationToken cancellationToken);

		Task DecrementFollowerCountAsync(Guid organizationId, CancellationToken cancellationToken);

	}
}
