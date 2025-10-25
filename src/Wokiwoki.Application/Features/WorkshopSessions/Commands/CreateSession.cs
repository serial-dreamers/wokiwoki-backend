using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Features.WorkshopSessions.Queries;
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
         Guid? ScheduleId,  // null = session riêng, không theo định kỳ

         bool IsActive = true
    ) : IRequest<WorkshopSession>;
    public class CreateSession : IRequestHandler<CreateSessionCommand, WorkshopSession>
    {
        private readonly IWorkshopSessionRepository _repo;
        private readonly IWorkshopRepository _workshopRepository;
        private readonly IWorkshopScheduleRepository _workshopScheduleRepository;
        private readonly IMapper _mapper;
        public CreateSession(IWorkshopSessionRepository repo, IWorkshopScheduleRepository workshopScheduleRepository, IWorkshopRepository workshopRepository, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
            _workshopRepository = workshopRepository;
            _workshopScheduleRepository = workshopScheduleRepository;
        }

        public Task<WorkshopSession> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                WorkshopSession session = new WorkshopSession();
                _mapper.Map(request, session);
                var result = _repo.CreateAsync(session);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}