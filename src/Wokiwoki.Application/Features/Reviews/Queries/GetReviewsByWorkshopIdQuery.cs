using AutoMapper;
using MediatR;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response; 

namespace Wokiwoki.Application.Features.Reviews.Queries
{
    public sealed record GetReviewsByWorkshopIdQuery(Guid WorkshopId, int PageNumber, int PageSize) : IRequest<PaginatedList<ReviewDto>>;

    public class GetReviewsByWorkshopIdQueryHandler : IRequestHandler<GetReviewsByWorkshopIdQuery, PaginatedList<ReviewDto>>
    {
        private readonly IReviewRepository _repo;
        private readonly IMapper _mapper;

        public GetReviewsByWorkshopIdQueryHandler(IReviewRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<PaginatedList<ReviewDto>> Handle(GetReviewsByWorkshopIdQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _repo.GetByWorkshopId(request.WorkshopId, request.PageNumber, request.PageSize, cancellationToken);

            var items = _mapper.Map<List<ReviewDto>>(reviews.Records);      

            return new PaginatedList<ReviewDto>(
                items,
                reviews.TotalCount,
                reviews.PageNumber,
                request.PageSize
            );
		}
    }
}
