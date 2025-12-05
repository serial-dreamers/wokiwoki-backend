using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
    public interface IWorkshopScheduleTicketRepository : IBaseRepo<WorkshopScheduleTicket, Guid>
    {
        Task<List<WorkshopScheduleTicket>> GetScheduleTicketBySchedulId(Guid schedulId);

	}
}
