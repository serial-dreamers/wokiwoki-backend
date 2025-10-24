using AutoMapper;
using MediatR;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Workshops.Queries.GetFilterPagedWorkshopsQuery
{
	public record SearchWorkshopQuery(
		Guid? CategoryId,
		List<Guid>? TagIds,
		string? DateFilterType,
		DateTime? StartDate,
		DateTime? EndDate,
		bool? IsFree,
		int? WorkshopTypeId,
		int PageNumber = 1,
		int PageSize = 10
		) : IRequest<PaginatedList<SearchWorkshopDto>>;

	public class SearchWorkshopQueryHandler : IRequestHandler<SearchWorkshopQuery, PaginatedList<SearchWorkshopDto>>
	{
		private readonly IWorkshopRepository _workshopRepository;
		private readonly IMapper _mapper;

		public SearchWorkshopQueryHandler(IWorkshopRepository workshopRepository, IMapper mapper)
		{
			_workshopRepository = workshopRepository;
			_mapper = mapper;
		}

		public async Task<PaginatedList<SearchWorkshopDto>> Handle(SearchWorkshopQuery request, CancellationToken cancellationToken)
		{ 
			var workshops = await _workshopRepository.SearchAsync(request, cancellationToken);
			var mappedItems = _mapper.Map<IReadOnlyCollection<SearchWorkshopDto>>(workshops.Records);

			return new PaginatedList<SearchWorkshopDto>(
				mappedItems,
			    workshops.TotalCount,
				workshops.PageNumber,
				request.PageSize
			);
		}
	}
}
