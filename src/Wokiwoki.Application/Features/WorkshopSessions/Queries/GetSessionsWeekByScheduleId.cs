using AutoMapper;
using MediatR; 
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.WorkshopSessions.Queries
{
	public record GetSessionsWeekByScheduleIdQuery(
		Guid ScheduleId,
		DateTime? StartTime = null,
		DateTime? EndTime = null
	) : IRequest<List<WorkshopSessionDto>>;

	public class GetSessionsWeekByScheduleIdQueryHandler : IRequestHandler<GetSessionsWeekByScheduleIdQuery, List<WorkshopSessionDto>>
	{
		private readonly IWorkshopSessionRepository _workshopSessionRepository;
		private readonly IMapper _mapper;

		public GetSessionsWeekByScheduleIdQueryHandler(IWorkshopSessionRepository workshopSessionRepository,
			IMapper mapper)
		{
			_workshopSessionRepository = workshopSessionRepository;
			_mapper = mapper;
		}

		public async Task<List<WorkshopSessionDto>> Handle(GetSessionsWeekByScheduleIdQuery request, CancellationToken cancellationToken)
		{
			var sessions = await _workshopSessionRepository.GetSessionsWeekByScheduleId(request.ScheduleId, request.StartTime, request.EndTime, cancellationToken);

			return _mapper.Map<List<WorkshopSessionDto>>(sessions);
		}
	}
}
