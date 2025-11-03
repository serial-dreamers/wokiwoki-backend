using AutoMapper;
using MediatR;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.Workshops.Queries.GetFilterPagedWorkshopsQuery
{
	public record GetWorkshopsByOrganizationFilterQuery(
			Guid OrganizationId,
			string? Title,
			WorkshopStatus? Status,
			int PageNumber,
			int PageSize
		) : IRequest<PaginatedList<WorkshopDto>>;

	public class GetWorkshopsByOrganizationFilterQueryHandler : IRequestHandler<GetWorkshopsByOrganizationFilterQuery, PaginatedList<WorkshopDto>>
	{
		private readonly IWorkshopRepository _workshopRepository;
		private readonly IMapper _mapper;
		public GetWorkshopsByOrganizationFilterQueryHandler(IWorkshopRepository workshopRepository, IMapper mapper)
		{
			_workshopRepository = workshopRepository;
			_mapper = mapper;

		}
		public async Task<PaginatedList<WorkshopDto>> Handle(GetWorkshopsByOrganizationFilterQuery request, CancellationToken cancellationToken)
		{
			var workshops = await _workshopRepository.GetByOrganizationWithFilterAsync(request.OrganizationId, request.Title, request.Status, request.PageNumber, request.PageSize, cancellationToken);

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
