using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.DTOs;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.WorkshopSessions.Commands
{
    public sealed record Create1MonthSessionCommand(
        Guid scheduleId,
        string Title,
        string Description,
        string? Street,
        string? Commune,
        string? Province,
        double? Latitude,
        double? Longitude,
        AgeRestrictionType AgeRestrictionType,
        int? MinimumAge,
        ParkingType? ParkingType,
        string? ParkingDescription
        ) : IRequest<List<CreatedDto>>;

    public class Create1MonthSession : IRequestHandler<Create1MonthSessionCommand, List<CreatedDto>>
    {
        private readonly IWorkshopSessionRepository _repo;
		private readonly IMapper _mapper;
        private readonly IGoongMapService _goongMapService;

		public Create1MonthSession (IWorkshopSessionRepository repo, IMapper mapper, IGoongMapService goongMapService)
        {
            _repo = repo; 
            _mapper = mapper;
            _goongMapService = goongMapService;
        }
        public async Task<List<CreatedDto>> Handle(Create1MonthSessionCommand request, CancellationToken cancellationToken)
        {
			var session = _mapper.Map<WorkshopSession>(request);
			
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
			
			var result = await _repo.Create1MonthSession(request.scheduleId, session, cancellationToken);

			return _mapper.Map<List<CreatedDto>>(result);
		}
    }
}
