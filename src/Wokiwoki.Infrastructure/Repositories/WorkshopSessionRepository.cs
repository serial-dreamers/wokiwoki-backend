using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Repositories;

namespace Wokiwoki.Infrastructure.Repositories
{
    public class WorkshopSessionRepository : BaseRepo<WorkshopSession, Guid>, IWorkshopSessionRepository
    {
    }
}
