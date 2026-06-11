using AutoMapper;
using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
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
        private readonly IUuidService _uuidService;
        private readonly IMapper _mapper;
        public CreateBooking(IBookingRepository bookingRepository, IMapper mapper, IWorkshopRepository workshopRepository, IWorkshopSessionRepository workshopSessionRepository, IUuidService uuidService)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
            _workshopRepository = workshopRepository;
            _workshopSessionRepository = workshopSessionRepository;
            _uuidService = uuidService;
        }

        public async Task<Booking> Handle(CreateBookingCommand command, CancellationToken cancellationToken)
        {
			if (command.Tickets == null || !command.Tickets.Any())
				throw new ArgumentException("Booking must include at least one ticket.");

			var workshop = await _workshopRepository.GetByIdAsync(command.WorkshopId);
            if (workshop == null)
            {
                return null;
            }
            var entity = new Booking();
            _mapper.Map(command, entity);
            entity.Id = _uuidService.NewGuid();
            entity.Status = BookingStatus.Pending;
            entity.FullName = command.FullName;
            entity.PhoneNumber = command.PhoneNumber;
            entity.Created = DateTime.UtcNow;
            entity.CreatedBy = command.UserId;
            foreach(var ticket in entity.Tickets)
            {
                ticket.Id = _uuidService.NewGuid(); 
                ticket.Created = DateTime.UtcNow;
                ticket.CreatedBy = command.UserId;
            }
            var result = await _bookingRepository.CreateAsync(entity, cancellationToken);
            if (result != null)
            {
                // Note: Both BookedCount (for sessions) and TotalBookings (for workshop)
                // should only be increased when payment is confirmed (in ConfirmBooking.cs)
                // This ensures we only count actual paid bookings
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
