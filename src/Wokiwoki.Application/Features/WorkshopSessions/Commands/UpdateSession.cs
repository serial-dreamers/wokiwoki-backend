using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.WorkshopSessions.Commands
{
    public sealed record UpdateSessionCommand(
        Guid Id,
        string Title,
        string Description,
        DateTime StartTime ,
        DateTime EndTime ,
        string? Street ,
        string? Commune ,
        string? Province ,
        double? Latitude ,
        double? Longitude ,
        AgeRestrictionType AgeRestrictionType ,
        int? MinimumAge ,
        ParkingType? ParkingType ,
        string? ParkingDescription ,
        int Capacity ,
        int BookedCount,
        bool IsActive
        ) : IRequest<WorkshopSession>;

    public class UpdateSession : IRequestHandler<UpdateSessionCommand, WorkshopSession>
    {
        private readonly IWorkshopSessionRepository _repo;
        private readonly IMapper _mapper;
        public UpdateSession(IWorkshopSessionRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<WorkshopSession> Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
        {
            var exist = await _repo.GetByIdAsync(request.Id);
            if (exist == null)
            {
                throw new NullReferenceException("Session not found!");
            }
            _mapper.Map(request, exist);
            return await _repo.UpdateTAsync(exist.Id, exist, cancellationToken);
        }
    }
}
