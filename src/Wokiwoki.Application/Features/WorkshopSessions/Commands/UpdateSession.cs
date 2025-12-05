using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.DTOs.Response;
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
        ) : IRequest<WorkshopSessionDto>;

    public class UpdateSession : IRequestHandler<UpdateSessionCommand, WorkshopSessionDto>
    {
        private readonly IWorkshopSessionRepository _repo;
        private readonly IMapper _mapper;
        private readonly IGoongMapService _goongMapService;
        
        public UpdateSession(IWorkshopSessionRepository repo, IMapper mapper, IGoongMapService goongMapService)
        {
            _repo = repo;
            _mapper = mapper;
            _goongMapService = goongMapService;
        }

        public async Task<WorkshopSessionDto> Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
        {
            var exist = await _repo.GetByIdAsync(request.Id);
            if (exist == null)
            {
                throw new NullReferenceException("Session not found!");
            }
             _mapper.Map(request, exist);

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
                    exist.Latitude = coordinates.Value.lat;
                    exist.Longitude = coordinates.Value.lng;
                }
            }

            var session =  await _repo.UpdateTAsync(exist.Id, exist, cancellationToken);

            return _mapper.Map<WorkshopSessionDto>(session);
        }
    }
}
