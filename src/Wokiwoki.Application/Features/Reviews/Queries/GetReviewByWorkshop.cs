using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.Reviews.Queries
{
    public sealed record GetReviewByWorkshopQuery(
        Guid workshopId,
        int pageNo = 1,
        int pageSize = 10
        ) : IRequest<PaginatedList<Review>>;
    public class GetReviewByWorkshop : IRequestHandler<GetReviewByWorkshopQuery, PaginatedList<Review>>
    {
        private readonly IReviewRepository _repo;
        private readonly IWorkshopRepository _workshopRepository;
        public GetReviewByWorkshop( IReviewRepository repo, IWorkshopRepository workshopRepository)
        {
            _repo = repo;
            _workshopRepository = workshopRepository;
        }
        public async Task<PaginatedList<Review>> Handle(GetReviewByWorkshopQuery request, CancellationToken cancellationToken)
        {
            var w = await _workshopRepository.GetByIdAsync(request.workshopId, cancellationToken);
            if (w == null)
            {
                return null;
            }
            var result = await _repo.GetByWorkshopId(request.workshopId, request.pageNo, request.pageSize, cancellationToken);
            return result;
        }
    }
}
