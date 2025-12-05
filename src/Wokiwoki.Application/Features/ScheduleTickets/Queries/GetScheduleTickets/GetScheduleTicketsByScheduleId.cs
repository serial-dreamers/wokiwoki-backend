using AutoMapper;
using MediatR;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.ScheduleTickets.Queries.GetScheduleTickets
{
	public record GetScheduleTicketsByScheduleIdQuery(
		Guid ScheduleId) : IRequest<List<WorkshopScheduleTicketDto>>;

	public class GetScheduleTicketsByScheduleIdQueryHandler : IRequestHandler<GetScheduleTicketsByScheduleIdQuery, List<WorkshopScheduleTicketDto>>
	{
		private readonly IWorkshopScheduleTicketRepository _workshopScheduleTicketRepository;
		private readonly IMapper _mapper;
		public GetScheduleTicketsByScheduleIdQueryHandler(IWorkshopScheduleTicketRepository workshopScheduleTicketRepository,
			IMapper mapper)
		{
			_workshopScheduleTicketRepository = workshopScheduleTicketRepository;
			_mapper = mapper;
		}
		public async Task<List<WorkshopScheduleTicketDto>> Handle(GetScheduleTicketsByScheduleIdQuery request, CancellationToken cancellationToken)
		{
			var scheduleTickets = await _workshopScheduleTicketRepository.GetScheduleTicketBySchedulId(request.ScheduleId);

			return _mapper.Map<List<WorkshopScheduleTicketDto>>(scheduleTickets);
		}
	}
}
