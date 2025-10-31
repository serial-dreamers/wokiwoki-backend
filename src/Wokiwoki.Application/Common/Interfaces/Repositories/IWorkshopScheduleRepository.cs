using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
    public interface IWorkshopScheduleRepository : IBaseRepo<WorkshopSchedule, Guid>
    {
        
    }
}
