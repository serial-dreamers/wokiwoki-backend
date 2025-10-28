using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Repositories;

namespace Wokiwoki.Infrastructure.Repositories
{
    public class BookingRepository : BaseRepo<Booking, Guid>, IBookingRepository
    {
        
        public new async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Bookings
                .Include(b => b.Workshop)
                .Include(b => b.Tickets) 
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }
    }
}
