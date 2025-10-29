using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Infrastructure.Repositories
{
    public class BookingRepository : BaseRepo<Booking, Guid>, IBookingRepository
    {
        public BookingRepository(WokiwokiDbContext context) : base(context)
        {
        }

        public new async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Bookings
                .Include(b => b.Workshop)
                .Include(b => b.Tickets)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }
        public async Task<bool> UpdateBookingStatus(Guid id, BookingStatus status, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity != null)
            {
                entity.Status = status;
            }
            await UpdateAsync(id, entity);
            await _context.SaveChangesAsync();
            var updated = await GetByIdAsync(id);
            return updated.Status == status;
        }
        public async Task<bool> CancleBooking(Guid id, CancellationToken cancellationToken = default)
        {
            return await UpdateBookingStatus(id, BookingStatus.Cancelled, cancellationToken);
        }
        public async Task<bool> CompleteBooking(Guid id, CancellationToken cancellationToken = default)
        {
            return await UpdateBookingStatus(id, BookingStatus.Completed, cancellationToken);
        }
        public async Task<bool> ConfirmBooking(Guid id, CancellationToken cancellationToken = default)
        {
            return await UpdateBookingStatus(id, BookingStatus.Confirmed, cancellationToken);
        }
        public async Task<bool> RefundBooking(Guid id, CancellationToken cancellationToken = default)
        {
            return await UpdateBookingStatus(id, BookingStatus.Refunded, cancellationToken);
        }
    }
}