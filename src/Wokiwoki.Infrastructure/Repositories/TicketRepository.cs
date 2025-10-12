using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Infrastructure.Repositories
{
    public class TicketRepository : BaseRepo<Ticket, Guid>
    {
        public TicketRepository(WokiwokiDbContext context) : base(context) { }

    }
}
