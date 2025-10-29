using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Application.Features.Bookings.Commands
{
    public sealed record ConfirmBookingCommand(
        Guid Id
        ) : IRequest<bool>;
    public class ConfirmBooking : IRequestHandler<ConfirmBookingCommand, bool>
    {
        private readonly IBookingRepository _repository;
        public ConfirmBooking( IBookingRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
        {
            return await _repository.ConfirmBooking(request.Id, cancellationToken);
        }
    }
}
