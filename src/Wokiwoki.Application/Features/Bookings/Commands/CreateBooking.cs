using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.Bookings.Commands
{
    public sealed record CreateBookingCommand(
        Guid UserId,
        Guid WorkshopId,
        decimal TotalPrice,
        List<TicketCreateDTO> Tickets
        ) : IRequest<Booking>;

    public class CreateBooking : IRequestHandler<CreateBookingCommand, Booking>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;
        public CreateBooking(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<Booking> Handle(CreateBookingCommand command, CancellationToken cancellationToken)
        {
            var entity = new Booking();
            _mapper.Map(command, entity);
            entity.Status = BookingStatus.Pending;
            var result = await _bookingRepository.CreateAsync(entity, cancellationToken);
            return result;  
        }
    }
    public class TicketCreateDTO
    {
        public Guid TicketTypeId { get; set; }

        public Guid SessionId { get; set; }

        public string QrCodeImage { get; set; } = null!;

        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
