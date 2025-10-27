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
        public async Task<Booking> GetByIdAsync(Guid id)
        {
            return await _context.Bookings.Include(b => b.Workshop).FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
