using AutoMapper;
using MediatR; 
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.WorkshopSessions.Queries
{
    public sealed record GetSessionByScheduleIdQuery(
        Guid ScheduleId
        ) : IRequest<List<WorkshopSession>>;
    public class GetSessionByScheduleId : IRequestHandler<GetSessionByScheduleIdQuery, List<WorkshopSession>>
    {
        private readonly IWorkshopSessionRepository _workshopSessionRepository;
        private readonly IMapper _mapper;

        public GetSessionByScheduleId(IWorkshopSessionRepository workshopSessionRepository,
            IMapper mapper)
        {
            _workshopSessionRepository = workshopSessionRepository;
            _mapper = mapper;
        }
        public async Task<List<WorkshopSession>> Handle(GetSessionByScheduleIdQuery request, CancellationToken cancellationToken)
        {
            var sessions= await _workshopSessionRepository.GetSessionByScheduleId(request.ScheduleId, cancellationToken);
			return _mapper.Map<List<WorkshopSession>>(sessions);
        }
    }
}
