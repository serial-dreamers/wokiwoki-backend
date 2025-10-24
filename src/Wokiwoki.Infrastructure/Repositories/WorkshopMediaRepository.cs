using Wokiwoki.Application.Common.Interfaces.Repositories;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class WorkshopMediaRepository : BaseRepo<WorkshopMedia, Guid>, IWorkshopMediaRepository
	{
		public WorkshopMediaRepository(WokiwokiDbContext context) : base(context)
		{
		}
	} 
}
