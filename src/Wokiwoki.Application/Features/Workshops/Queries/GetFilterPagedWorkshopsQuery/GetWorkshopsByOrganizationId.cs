using AutoMapper;
using MediatR; 
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Workshops.Queries.GetFilterPagedWorkshopsQuery
{
	public record GetWorkshopsByOrganizationIdQuery(
		Guid organizationId,
		int PageNumber = 1,
		int PageSize = 10
		) : IRequest<PaginatedList<WorkshopDto>>;

	public class GetWorkshopsByOrganizationIdQueryHandler : IRequestHandler<GetWorkshopsByOrganizationIdQuery, PaginatedList<WorkshopDto>>
	{
		private readonly IWorkshopRepository _workshopRepository;
		private readonly IMapper _mapper;
		public GetWorkshopsByOrganizationIdQueryHandler(IWorkshopRepository workshopRepository, IMapper mapper)
		{
			_workshopRepository = workshopRepository;
			_mapper = mapper;
		}
		public async Task<PaginatedList<WorkshopDto>> Handle(GetWorkshopsByOrganizationIdQuery request, CancellationToken cancellationToken)
		{
			var workshops = await _workshopRepository.GetWorkshopByWorkshopIdAsync(request.organizationId, request.PageNumber, request.PageSize, cancellationToken);

			foreach (var w in workshops.Records)
			{
				w.Description = string.Empty;
			}

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
