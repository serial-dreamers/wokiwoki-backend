using AutoMapper;
using MediatR; 
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs;

namespace Wokiwoki.Application.Features.Workshops.Queries.GetFilterPagedWorkshopsQuery
{
	public record SearchWorkshopQuery(
		Guid? cateId,
		List<Guid>? tagIdList,
		string? typeDate,
		DateTime? startDate,
		DateTime? endDate,
		bool? isFree,
		Guid? typeId,
		int pageNo = 1,
		int pageSize = 10
		) : IRequest<PaginatedList<SearchWorkshopDto>>;

	public class SearchWorkshopQueryHandler : IRequestHandler<SearchWorkshopQuery, PaginatedList<SearchWorkshopDto>>
	{
		private readonly IWorkshopRepository _workshopRepository;
		private IMapper _mapper;

		public SearchWorkshopQueryHandler(IWorkshopRepository workshopRepository, IMapper mapper)
		{
			_workshopRepository = workshopRepository;
			_mapper = mapper;
		}

		public async Task<PaginatedList<SearchWorkshopDto>> Handle(SearchWorkshopQuery request, CancellationToken cancellationToken)
		{
			var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
			DateTime vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

			var workshops = await _workshopRepository.SearchAsync(request, cancellationToken);
			var mappedItems = _mapper.Map<IReadOnlyCollection<SearchWorkshopDto>>(workshops.Records);

			return new PaginatedList<SearchWorkshopDto>(
				mappedItems,
			    workshops.TotalCount,
				workshops.PageNumber,
				request.pageSize
			);
		}
	}
}
