using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.Bookings.Commands
{
    public sealed record UpdateBookingStatusCommand(
        Guid Id,
        int BookingStatus
        ) : IRequest<bool>;
    public class UpdateBookingStatus : IRequestHandler<UpdateBookingStatusCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        public UpdateBookingStatus(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        public Task<bool> Handle(UpdateBookingStatusCommand request, CancellationToken cancellationToken)
        {
            BookingStatus status = (BookingStatus) request.BookingStatus;
            return _bookingRepository.UpdateBookingStatus(request.Id, status, cancellationToken);
        }
    }
}
