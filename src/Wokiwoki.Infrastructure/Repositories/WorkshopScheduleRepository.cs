using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Infrastructure.Data.Extensions;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class WorkshopScheduleRepository : BaseRepo<WorkshopSchedule, Guid>, IWorkshopScheduleRepository
    {
        public WorkshopScheduleRepository(WokiwokiDbContext context) : base(context)
        {
        }

		public async Task<PaginatedList<WorkshopSchedule>> GetSchedulesByWorkshopId(Guid workshopId,int pageNumber, int pageSize, CancellationToken cancellationToken)
		{
			return await _context.WorkshopSchedules.Where(wsc => wsc.WorkshopId == workshopId).OrderByDescending(wsc => wsc.Id).ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
		}
		//public async Task<PaginatedList<WorkshopSchedule>> GetByWorkshopId(Guid workshopId)
		//{

		//}
	}
}
