
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Domain.Entities; 

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
    public interface IWorkshopScheduleRepository : IBaseRepo<WorkshopSchedule, Guid>
    {
        Task<PaginatedList<WorkshopSchedule>> GetSchedulesByWorkshopId(Guid workshopId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}
