using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.Bookings.Queries
{
    public sealed record GetBookingByIdQuery
    (
         Guid Id
    ) : IRequest<Booking>;

    public class GetBookingById : IRequestHandler<GetBookingByIdQuery, Booking>
    {
        private readonly IBookingRepository _repository;

        public GetBookingById(IBookingRepository repository)
        {
            _repository = repository;
        }

        public async Task<Booking> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (result == null)
            {
                throw new Exception("Booking not found");
            }
            return result;
        }
    }
}
