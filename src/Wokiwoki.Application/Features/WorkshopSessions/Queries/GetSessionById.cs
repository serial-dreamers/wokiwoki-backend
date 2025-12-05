using AutoMapper;
using MediatR; 
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.WorkshopSessions.Queries
{
    public sealed record GetSessionByIdQuery(Guid id) : IRequest<WorkshopSession>;
    public class GetSessionById : IRequestHandler<GetSessionByIdQuery, WorkshopSession>
    {
        private readonly IWorkshopSessionRepository _repo;
        //private readonly IWorkshopRepository _workshopRepository;
        private readonly IMapper _mapper;

        public GetSessionById(IWorkshopSessionRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<WorkshopSession> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
        {
            // 1️⃣ Lấy workshop hiện có (draft)
            var session = await _repo.GetByIdAsync(request.id);
            if (session == null)
                throw new Exception("Sesion not found");

            return _mapper.Map<WorkshopSession>(session);
        }
    }
}
