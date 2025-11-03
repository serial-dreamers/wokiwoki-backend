using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.Bookings.Queries
{
    public sealed record GetBookingByTimeQuery(
        DateTime time, 
        int pageNo, 
        int pageSize
        ) : IRequest<PaginatedList<Booking>>;
    public class GetBookingByTime : IRequestHandler<GetBookingByTimeQuery, PaginatedList<Booking>>
    {
        private readonly IBookingRepository _repo;
        public GetBookingByTime( IBookingRepository repo)
        {
            _repo = repo;
        }
        public async Task<PaginatedList<Booking>> Handle(GetBookingByTimeQuery request, CancellationToken cancellationToken)
        {
            var result = await _repo.GetBookingByMonth(request.time, request.pageNo, request.pageSize);
            return result;
        }
    }
}
