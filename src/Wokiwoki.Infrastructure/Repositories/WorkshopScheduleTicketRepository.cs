using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Common.Interfaces.Repositories;


namespace Wokiwoki.Infrastructure.Repositories
{
    public class WorkshopScheduleTicketRepository : BaseRepo<WorkshopScheduleTicket, Guid>, IWorkshopScheduleTicketRepository
    {
        public WorkshopScheduleTicketRepository(WokiwokiDbContext context) : base(context)
        {
        }

		public async Task<List<WorkshopScheduleTicket>> GetScheduleTicketBySchedulId(Guid schedulId)
		{
			return await _context.WorkshopScheduleTickets.Where(wst => wst.WorkshopScheduleId == schedulId).ToListAsync();
		}
	}
}
