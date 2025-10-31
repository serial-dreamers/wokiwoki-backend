using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
    public interface IWorkshopScheduleTicketRepository : IBaseRepo<WorkshopScheduleTicket, Guid>
    {
    }
}
