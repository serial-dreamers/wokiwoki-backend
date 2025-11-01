using AutoMapper;
using MediatR; 
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.WorkshopSchedules.Commands.CreateSchedule
{
    public record CreateScheduleCommand(
        Guid WorkshopId,
        RecurrenceType RecurrenceType,
        string? DaysOfWeek,
        string? DaysOfMonth,
        TimeOnly StartTime,
        TimeOnly EndTime,
        DateTime ValidFrom,
        DateTime? ValidUntil,
        int? Capacity
	) : IRequest<Guid>;

    public class CreateScheduleCommandHandler : IRequestHandler<CreateScheduleCommand, Guid>
    {
        private readonly IWorkshopScheduleRepository _repo;
        private readonly IWorkshopRepository _workshopRepository;

        private readonly IMapper _mapper;
        private readonly IUuidService _uuidService;

        public CreateScheduleCommandHandler(IWorkshopScheduleRepository repo, IWorkshopRepository workshopRepository, IMapper mapper, IUuidService uuidService)
        {
            _repo = repo;
            _workshopRepository = workshopRepository;
            _mapper = mapper;
            _uuidService = uuidService;
        }
        public async Task<Guid> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
        { 
            var workshop = await _workshopRepository.GetByIdAsync(request.WorkshopId);

            if (workshop == null)
                throw new Exception("Workshop not found");
            var schedule = new WorkshopSchedule();

            _mapper.Map(request, schedule);
                
           schedule.Created= DateTime.UtcNow;
            var result = await _repo.CreateAsync(schedule);

            if (result == null)
                throw new Exception("Create schedule failed");
             
            return result.Id;
        }
    }
}
