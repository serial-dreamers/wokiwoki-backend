using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Infrastructure.Repositories
{
    public class TicketRepository : BaseRepo<Ticket, Guid>, ITicketRepository
	{
        public TicketRepository(WokiwokiDbContext context) : base(context) { }

        public async Task<Ticket?> GetTicketWithDetailsAsync(Guid ticketId, CancellationToken cancellationToken = default)
        {
            return await _context.Tickets
                .AsNoTracking()
                .Include(t => t.WorkshopSession)
                .Include(t => t.TicketType)
                .Include(t => t.Booking)
                    .ThenInclude(b => b.Workshop)
                        .ThenInclude(w => w.Organization)
                .FirstOrDefaultAsync(t => t.Id == ticketId, cancellationToken);
        }

        public async Task<Result> CheckInTicketAsync(Guid ticketId, CancellationToken cancellationToken = default)
        {
            // Get ticket with related data
            var ticket = await _context.Tickets
                .Include(t => t.Booking)
                    .ThenInclude(b => b.Workshop)
                        .ThenInclude(w => w.Organization)
                .FirstOrDefaultAsync(t => t.Id == ticketId && t.IsActive, cancellationToken);

            if (ticket == null)
                return Result.Failure(new[] { "Ticket not found" });
             
            if (ticket.IsCheckedIn)
                return Result.Failure(new[] { "This ticket has already been checked in" });
             
            if ((int)ticket.Booking.Status != 1) // 1 = Confirmed
                return Result.Failure(new[] { "Cannot check-in: booking is not confirmed" });

            // Perform check-in
            ticket.IsCheckedIn = true;
            ticket.CheckedInAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

    public async Task<PaginatedList<ParticipantDto>> GetOrganizerParticipantsAsync(
        string userId,
        Guid? workshopId,
        Guid? sessionId,
        DateTime? startDate,
        DateTime? endDate,
        bool? isCheckedIn,
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        {
            // Get organizer's organization IDs
            var organizationIds = await _context.Organizations
                .Where(o => o.OwnerId == userId)
                .Select(o => o.Id)
                .ToListAsync(cancellationToken);

            if (!organizationIds.Any())
                return PaginatedList<ParticipantDto>.Create(new List<ParticipantDto>(), 0, pageNumber, pageSize);

            // Build query
            var query = _context.Tickets
                .Include(t => t.Booking)
                    .ThenInclude(b => b.Workshop)
                        .ThenInclude(w => w.Organization)
                .Include(t => t.WorkshopSession)
                .Include(t => t.TicketType)
                .Where(t => organizationIds.Contains(t.Booking.Workshop.OrganizationId))
                .Where(t => t.IsActive)
                .Where(t => t.Booking.IsActive)
                // Only show confirmed bookings (status = 1) and completed (status = 3)
                .Where(t => (int)t.Booking.Status == 1 || (int)t.Booking.Status == 3)
                .AsQueryable();

            // Apply filters
            if (workshopId.HasValue)
            {
                query = query.Where(t => t.Booking.WorkshopId == workshopId.Value);
            }

            if (sessionId.HasValue)
            {
                query = query.Where(t => t.SessionId == sessionId.Value);
            }

        // Apply date range filter with UTC conversion
        if (startDate.HasValue)
        {
            var filterStartDate = startDate.Value.Kind == DateTimeKind.Utc 
                ? startDate.Value 
                : DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);
            query = query.Where(t => t.WorkshopSession.StartTime >= filterStartDate);
        }

        if (endDate.HasValue)
        {
            var filterEndDate = endDate.Value.Kind == DateTimeKind.Utc 
                ? endDate.Value 
                : DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);
            query = query.Where(t => t.WorkshopSession.StartTime <= filterEndDate);
        }

            if (isCheckedIn.HasValue)
            {
                query = query.Where(t => t.IsCheckedIn == isCheckedIn.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(t =>
                    t.Booking.FullName.ToLower().Contains(term) ||
                    t.Booking.PhoneNumber.Contains(term));
            }

            // Order by session start time (newest first), then by booking created date
            query = query.OrderByDescending(t => t.WorkshopSession.StartTime)
                         .ThenByDescending(t => t.Booking.Created);

            // Get total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply paging and project to DTO
            var participants = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new ParticipantDto
                {
                    TicketId = t.Id,
                    BookingId = t.BookingId,
                    FullName = t.Booking.FullName ?? "N/A",
                    PhoneNumber = t.Booking.PhoneNumber ?? "N/A",
                    Quantity = t.Quantity,
                    TicketTypeName = t.TicketType.Name,
                    Price = t.Price,
                    IsCheckedIn = t.IsCheckedIn,
                    CheckedInAt = t.CheckedInAt,
                    BookingDate = t.Booking.Created,
                    WorkshopId = t.Booking.WorkshopId,
                    WorkshopTitle = t.Booking.Workshop.Title,
                    WorkshopImageUrl = t.Booking.Workshop.ImageUrl,
                    SessionId = t.SessionId,
                    SessionTitle = t.WorkshopSession.Title,
                    SessionStartTime = t.WorkshopSession.StartTime,
                    SessionEndTime = t.WorkshopSession.EndTime,
                    SessionLocation = !string.IsNullOrEmpty(t.WorkshopSession.Street)
                        ? $"{t.WorkshopSession.Street}, {t.WorkshopSession.Commune ?? ""}"
                        : t.Booking.Workshop.DisplayAddress,
                    BookingStatus = (int)t.Booking.Status
                })
                .ToListAsync(cancellationToken);

            return PaginatedList<ParticipantDto>.Create(participants, totalCount, pageNumber, pageSize);
        }
    }
}
