using AutoMapper;
using MediatR; 
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.DTOs.Response; 
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.WorkshopSessions.Commands
{
    public sealed record CreateSessionCommand
    (
         string Title,
         string Description,
         DateTime StartTime,
         DateTime EndTime,
         string? Street,
         string? Commune,
         string? Province,
         double? Latitude,
         double? Longitude,
         AgeRestrictionType AgeRestrictionType,
         int? MinimumAge,
         ParkingType? ParkingType,
         string? ParkingDescription,
         int Capacity,
         Guid WorkshopId,
         Guid? ScheduleId

    ) : IRequest<WorkshopSessionDto>;
    public class CreateSession : IRequestHandler<CreateSessionCommand, WorkshopSessionDto>
    {
        private readonly IWorkshopSessionRepository _repo; 
        private readonly IMapper _mapper;
        private readonly IUuidService _uuidService;
        public CreateSession(IWorkshopSessionRepository repo
            , IMapper mapper, IUuidService uuidService)
        {
            _mapper = mapper;
            _repo = repo; 
            _uuidService = uuidService;
        }

        public async Task<WorkshopSessionDto> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                WorkshopSession session = new WorkshopSession();
                _mapper.Map(request, session);
                session.Id = _uuidService.NewGuid();
                session.Created = DateTime.UtcNow;
                session.IsActive = true;

				var result = await _repo.CreateAsync(session);
                return _mapper.Map<WorkshopSessionDto>(result); 

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}