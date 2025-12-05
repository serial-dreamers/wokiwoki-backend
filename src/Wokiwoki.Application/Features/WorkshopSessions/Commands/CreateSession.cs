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
        private readonly IGoongMapService _goongMapService;
        
        public CreateSession(IWorkshopSessionRepository repo
            , IMapper mapper, IUuidService uuidService, IGoongMapService goongMapService)
        {
            _mapper = mapper;
            _repo = repo; 
            _uuidService = uuidService;
            _goongMapService = goongMapService;
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

                // Auto-fetch coordinates if address is provided and coordinates are missing
                if (!string.IsNullOrWhiteSpace(request.Street) && 
                    !string.IsNullOrWhiteSpace(request.Commune) && 
                    !string.IsNullOrWhiteSpace(request.Province) &&
                    (request.Latitude == null || request.Latitude == 0 || request.Longitude == null || request.Longitude == 0))
                {
                    var address = $"{request.Street}, {request.Commune}, {request.Province}";
                    var coordinates = await _goongMapService.GetCoordinatesAsync(address);
                    if (coordinates.HasValue)
                    {
                        session.Latitude = coordinates.Value.lat;
                        session.Longitude = coordinates.Value.lng;
                    }
                }

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