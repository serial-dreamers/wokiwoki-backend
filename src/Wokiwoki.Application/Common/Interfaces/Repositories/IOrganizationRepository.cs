using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
	public interface IOrganizationRepository : IBaseRepo<Organization, Guid>
	{
		Task<Guid?> GetOrganizationIdByUserIdAsync(string userId);
	}
}
