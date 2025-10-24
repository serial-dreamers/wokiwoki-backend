using Wokiwoki.Application.Common.Interfaces.Repositories;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class OrganizationRepository : BaseRepo<Organization, Guid>, IOrganizationRepository
	{
		public OrganizationRepository(WokiwokiDbContext context) : base(context)
		{
		}
	}
}
