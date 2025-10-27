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
        Guid WorkshopId,

        decimal TotalPrice
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
            var result = await _bookingRepository.CreateAsync(entity, cancellationToken);
            return result;
        }
    }
}
