using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
    public interface IWorkshopSessionRepository : IBaseRepo<WorkshopSession, Guid>
    {
        Task<List<WorkshopSession>> Create1MonthSession(Guid scheduleId, WorkshopSession session, CancellationToken cancellationToken = default);

    }
}
