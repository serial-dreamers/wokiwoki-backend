using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Infrastructure.Repositories
{
    public class WorkshopScheduleRepository : BaseRepo<WorkshopSchedule, Guid>, IWorkshopScheduleRepository
    {
        public WorkshopScheduleRepository(WokiwokiDbContext context) : base(context)
        {
        }
        //public async Task<PaginatedList<WorkshopSchedule>> GetByWorkshopId(Guid workshopId)
        //{

        //}
    }
}
