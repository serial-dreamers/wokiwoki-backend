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

		public Create1MonthSession (IWorkshopSessionRepository repo, IMapper mapper)
        {
            _repo = repo; 
            _mapper = mapper;
        }
        public async Task<List<CreatedDto>> Handle(Create1MonthSessionCommand request, CancellationToken cancellationToken)
        {
			var session = _mapper.Map<WorkshopSession>(request);
			var result = await _repo.Create1MonthSession(request.scheduleId, session, cancellationToken);

			return _mapper.Map<List<CreatedDto>>(result);
		}
    }
}
