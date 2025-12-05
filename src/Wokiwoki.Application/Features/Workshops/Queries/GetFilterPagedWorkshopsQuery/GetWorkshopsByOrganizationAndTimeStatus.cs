using AutoMapper;
using MediatR;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Workshops.Queries.GetFilterPagedWorkshopsQuery
{
	public record GetWorkshopsByOrganizationAndTimeStatusQuery(
		Guid OrganizationId,
		int TimeStatus, // 0: all, 1: upcoming, 2: ongoing, 3: completed
		int PageNumber,
		int PageSize
	) : IRequest<PaginatedList<WorkshopDto>>;

	public class GetWorkshopsByOrganizationAndTimeStatusQueryHandler : IRequestHandler<GetWorkshopsByOrganizationAndTimeStatusQuery, PaginatedList<WorkshopDto>>
	{
		private readonly IWorkshopRepository _workshopRepository;
		private readonly IMapper _mapper;

		public GetWorkshopsByOrganizationAndTimeStatusQueryHandler(IWorkshopRepository workshopRepository, IMapper mapper)
		{
			_workshopRepository = workshopRepository;
			_mapper = mapper;
		}

		public async Task<PaginatedList<WorkshopDto>> Handle(GetWorkshopsByOrganizationAndTimeStatusQuery request, CancellationToken cancellationToken)
		{
			var workshops = await _workshopRepository.GetWorkshopsByOrganizationAndTimeStatusAsync(
				request.OrganizationId,
				request.TimeStatus,
				request.PageNumber,
				request.PageSize,
				cancellationToken);

			var mappedItems = _mapper.Map<IReadOnlyCollection<WorkshopDto>>(workshops.Records);

			return new PaginatedList<WorkshopDto>(
				mappedItems,
				workshops.TotalCount,
				workshops.PageNumber,
				request.PageSize
			);
		}
	}
}

