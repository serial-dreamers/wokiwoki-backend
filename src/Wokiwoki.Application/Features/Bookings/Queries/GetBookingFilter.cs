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
    public sealed record GetBookingFilterQuery(
        DateTime time, 
        Guid? organizerId, 
        Guid? categoryId, 
        Guid? tagId, 
        int pageNo, 
        int pageSize
        ) : IRequest<PaginatedList<Booking>>;
    public class GetBookingFilter : IRequestHandler<GetBookingFilterQuery, PaginatedList<Booking>>
    {
        private readonly IBookingRepository _bookingRepository;
        public GetBookingFilter( IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        public Task<PaginatedList<Booking>> Handle(GetBookingFilterQuery request, CancellationToken cancellationToken)
        {
            var result = _bookingRepository.GetBookingByMonthAndOrganizer(request.time, request.organizerId, request.categoryId, request.tagId, request.pageNo, request.pageSize, cancellationToken);
            return result;
        }
    }
}
