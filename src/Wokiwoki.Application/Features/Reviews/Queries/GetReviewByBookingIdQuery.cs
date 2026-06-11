using AutoMapper;
using MediatR;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.Reviews.Queries
{
    public sealed record GetReviewByBookingIdQuery(Guid BookingId) : IRequest<ReviewDto?>;

    public class GetReviewByBookingIdQueryHandler : IRequestHandler<GetReviewByBookingIdQuery, ReviewDto?>
    {
        private readonly IReviewRepository _repo;
        private readonly IMapper _mapper;

		public GetReviewByBookingIdQueryHandler(IReviewRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
		}

        public async Task<ReviewDto?> Handle(GetReviewByBookingIdQuery request, CancellationToken cancellationToken)
        {
            var review = await _repo.GetByBookingIdAsync(request.BookingId, cancellationToken);
			return _mapper.Map<ReviewDto?>(review);
        }
    }
}
