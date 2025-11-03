using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Domain.Enums;
using Wokiwoki.Infrastructure.Data.Extensions;

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
        public async Task<PaginatedList<Booking>> GetBookingByMonth(
    DateTime time, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Bookings
                .AsNoTracking() // ✅ Không giữ context tracking, giảm nguy cơ ObjectDisposedException
                .Where(b => b.Created.Month == time.Month
                         && b.Created.Year == time.Year
                         && b.Status == BookingStatus.Confirmed)
                .Include(b => b.Workshop);

            // ✅ Thực thi truy vấn ngay trong cùng scope (không deferred)
            return await query.ToPaginatedListAsync(pageNo, pageSize, cancellationToken);
        }

        public async Task<PaginatedList<Booking>> GetBookingByMonthAndOrganizer(DateTime time, Guid organizerId, Guid? categoryId, Guid? tagId, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var bL = _context.Bookings.Where(b => b.Created.Month == time.Month && b.Created.Year == time.Year && b.Status == BookingStatus.Confirmed && b.Tickets.All(t => t.WorkshopSession.Workshop.OrganizationId == organizerId)).Include(b => b.Workshop).AsQueryable();
            if (categoryId != null)
            {
                bL = bL.Where(b => b.Tickets.All(b => b.WorkshopSession.Workshop.CategoryId == categoryId));
            }
            if (tagId != null)
            {
                bL = bL.Where(b => b.Tickets.All(b => b.WorkshopSession.Workshop.Tags.All(t => t.Id == tagId)));
            }
            var result = await bL.ToPaginatedListAsync(pageNo, pageSize, cancellationToken);
            return result;
        }
    }
}