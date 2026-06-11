using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.Bookings.Queries
{
    /// <summary>
    /// Get all bookings (tickets) for a specific user
    /// Only returns Confirmed and Completed bookings
    /// </summary>
    public sealed record GetUserBookingsQuery(
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<PaginatedList<Booking>>;

    public class GetUserBookings : IRequestHandler<GetUserBookingsQuery, PaginatedList<Booking>>
    {
        private readonly IBookingRepository _repository;
		private readonly IUserContext _userContext;


		public GetUserBookings(IBookingRepository repository, IUserContext userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        public async Task<PaginatedList<Booking>> Handle(GetUserBookingsQuery request, CancellationToken cancellationToken)
        {
            // Get only Confirmed (1) and Completed (3) bookings
            var result = await _repository.GetUserBookings(_userContext.UserId, request.PageNumber, request.PageSize, cancellationToken);
            return result;
        }
    }
}

