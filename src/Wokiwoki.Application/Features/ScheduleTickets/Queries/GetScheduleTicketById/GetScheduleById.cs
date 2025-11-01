using AutoMapper;
using MediatR; 
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.ScheduleTickets.Queries.GetScheduleTicketById
{
	public record GetScheduleTicketByIdQuery(Guid scheduleId) : IRequest<WorkshopScheduleTicketDto>;

	public class GetScheduleByIdQueryHandler : IRequestHandler<GetScheduleTicketByIdQuery, WorkshopScheduleTicketDto>
	{
		private readonly IWorkshopScheduleTicketRepository _workshopScheduleTicketRepository;
		private readonly IMapper _mapper;

		public GetScheduleByIdQueryHandler(IWorkshopScheduleTicketRepository workshopScheduleTicketRepository,
			IMapper mapper)
		{
			_workshopScheduleTicketRepository = workshopScheduleTicketRepository;
			_mapper = mapper;
		}
		public async Task<WorkshopScheduleTicketDto> Handle(GetScheduleTicketByIdQuery request, CancellationToken cancellationToken)
		{
			var scheduleTicket =  await _workshopScheduleTicketRepository.GetByIdAsync(request.scheduleId, cancellationToken);

			return _mapper.Map<WorkshopScheduleTicketDto>(scheduleTicket);
		}
	}
}
