using AutoMapper;
using MediatR; 
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.Bookings.Commands
{
    public sealed record CreateBookingCommand(
        string UserId,
        Guid WorkshopId,
        decimal TotalPrice,
        string FullName,
        string PhoneNumber,
		List<TicketCreateDTO> Tickets
        ) : IRequest<Booking>;

    public class CreateBooking : IRequestHandler<CreateBookingCommand, Booking>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IWorkshopRepository _workshopRepository;
        private readonly IWorkshopSessionRepository _workshopSessionRepository;
        private readonly IMapper _mapper;
        public CreateBooking(IBookingRepository bookingRepository, IMapper mapper, IWorkshopRepository workshopRepository, IWorkshopSessionRepository workshopSessionRepository)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
            _workshopRepository = workshopRepository;
            _workshopSessionRepository = workshopSessionRepository;
        }

        public async Task<Booking> Handle(CreateBookingCommand command, CancellationToken cancellationToken)
        {
            var workshop = await _workshopRepository.GetByIdAsync(command.WorkshopId);
            if (workshop == null)
            {
                return null;
            }
            var entity = new Booking();
            _mapper.Map(command, entity);
            entity.Status = BookingStatus.Pending;
            entity.FullName = command.FullName;
            entity.PhoneNumber = command.PhoneNumber;
            entity.Created = DateTime.UtcNow;
            entity.CreatedBy = command.UserId;
            foreach(var ticket in entity.Tickets)
            {
                ticket.Created = DateTime.UtcNow;
                ticket.CreatedBy = command.UserId;
            }
            var result = await _bookingRepository.CreateAsync(entity, cancellationToken);
            if (result != null)
            {
                foreach (var ticket in command.Tickets)
                {
                    var session = await _workshopSessionRepository.GetByIdAsync(ticket.SessionId);
                    if (session != null)
                    {
                        session.BookedCount++;
                        await _workshopSessionRepository.UpdateAsync(session.Id, session);
                    }
                }
                workshop.TotalBookings++;
                await _workshopRepository.UpdateAsync(command.WorkshopId, workshop);
            }
            return result;
        }
    }
    public class TicketCreateDTO
    {
        public Guid TicketTypeId { get; set; }
        public int Quantity { get; set; }

        public Guid SessionId { get; set; }

        //public string QrCodeImage { get; set; } = null!;

        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
