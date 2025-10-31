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
        ) : IRequest<List<WorkshopSession>>;
    public class Create1MonthSession : IRequestHandler<Create1MonthSessionCommand, List<WorkshopSession>>
    {
        private readonly IWorkshopSessionRepository _repo;
        public Create1MonthSession (IWorkshopSessionRepository repo)
        {
            _repo = repo;
        }
        public async Task<List<WorkshopSession>> Handle(Create1MonthSessionCommand request, CancellationToken cancellationToken)
        {
            var session = new WorkshopSession
            {
                Title = request.Title,
                Description = request.Description,
                Street = request.Street,
                Commune = request.Commune,
                Province = request.Province,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                MinimumAge = request.MinimumAge,
                ParkingType = request.ParkingType,
                ParkingDescription = request.ParkingDescription,
                AgeRestrictionType = request.AgeRestrictionType
            };
            var result = await _repo.Create1MonthSession(request.scheduleId, session, cancellationToken);
            return result;
        }
    }
}
