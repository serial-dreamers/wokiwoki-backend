using AutoMapper;
using MediatR; 
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response; 

namespace Wokiwoki.Application.Features.WorkshopSchedules.Queries.GetSchedule
{
	public record GetSchedulesByWorkshopIdQuery(
		Guid workshopId,
		int PageNumber = 1,
		int PageSize = 10
		): IRequest<PaginatedList<WorkshopScheduleDto>>;

	public class GetScheduleByWorkshopIdQueryHandler : IRequestHandler<GetSchedulesByWorkshopIdQuery, PaginatedList<WorkshopScheduleDto>>
	{
		private readonly IWorkshopScheduleRepository _workshopScheduleRepository;
		private readonly IMapper _mapper;

		public GetScheduleByWorkshopIdQueryHandler(IWorkshopScheduleRepository workshopScheduleRepository,
			IMapper mapper)
		{
			_workshopScheduleRepository = workshopScheduleRepository;
			_mapper = mapper;
		}

		public async Task<PaginatedList<WorkshopScheduleDto>> Handle(GetSchedulesByWorkshopIdQuery request, CancellationToken cancellationToken)
		{
			var schedules = await _workshopScheduleRepository.GetSchedulesByWorkshopId(request.workshopId,request.PageNumber, request.PageSize, cancellationToken);
			var mappedItems = _mapper.Map<IReadOnlyCollection<WorkshopScheduleDto>>(schedules.Records);

			return new PaginatedList<WorkshopScheduleDto>(
				mappedItems,
				schedules.TotalCount,
				schedules.PageNumber,
				request.PageSize
			);
		}
	}
}
