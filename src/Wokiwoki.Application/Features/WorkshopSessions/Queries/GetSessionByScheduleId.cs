using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.WorkshopSessions.Queries
{
    public sealed record GetSessionByScheduleIdQuery(
        Guid ScheduleId
        ) : IRequest<List<WorkshopSession>>;
    public class GetSessionByScheduleId : IRequestHandler<GetSessionByScheduleIdQuery, List<WorkshopSession>>
    {
        private readonly IWorkshopSessionRepository _workshopSessionRepository;
        public GetSessionByScheduleId(IWorkshopSessionRepository workshopSessionRepository)
        {
            _workshopSessionRepository = workshopSessionRepository;
        }
        public async Task<List<WorkshopSession>> Handle(GetSessionByScheduleIdQuery request, CancellationToken cancellationToken)
        {
            return await _workshopSessionRepository.GetSessionByScheduleId(request.ScheduleId, cancellationToken);
        }
    }
}
