using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs;

namespace Wokiwoki.Application.Features.Workshops.Queries.GetFilterPagedWorkshopsQuery
{
    public record GetAllWorkshopQuery(
        int pageNo,
        int pageSize
        ) : IRequest<PaginatedList<SearchWorkshopDto>>;
    public class GetAllWorkshopQueryHandler : IRequestHandler<GetAllWorkshopQuery, PaginatedList<SearchWorkshopDto>>
    {
        private readonly IWorkshopRepository _repo;
        private IMapper _mapper;
        public GetAllWorkshopQueryHandler(IWorkshopRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<PaginatedList<SearchWorkshopDto>> Handle(GetAllWorkshopQuery request, CancellationToken cancellationToken)
        {
            var wL = await _repo.GetAllAsync(cancellationToken);
            var wLMapped = _mapper.Map<List<SearchWorkshopDto>>(wL);
            var wLPaging = new PaginatedList<SearchWorkshopDto>(wLMapped, wLMapped.Count, request.pageNo, request.pageSize);
            return wLPaging;
        }
    }
}
