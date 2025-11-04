using AutoMapper;
using MediatR; 
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.WorkshopSchedules.Commands.CreateSchedule
{
    public record CreateScheduleCommand(
        Guid WorkshopId,
        RecurrenceType RecurrenceType,
        string? DaysOfWeek,
        string? DaysOfMonth,
        string StartTime,
        string EndTime,
        DateTime ValidFrom,
        DateTime? ValidUntil,
        int? Capacity
	) : IRequest<Result<Guid>>;

    public class CreateScheduleCommandHandler : IRequestHandler<CreateScheduleCommand, Result<Guid>>
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
        public async Task<Result<Guid>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
        { 
            var workshop = await _workshopRepository.GetByIdActiveAsync(request.WorkshopId);

            //        if (workshop == null)
            //return Result<Guid>.Failure(new[] { "Workshop not found" });

            var schedule = _mapper.Map<WorkshopSchedule>(request);
            schedule.WorkshopId = request.WorkshopId; // đảm bảo không bị Guid.Empty
            schedule.Created = DateTime.UtcNow;
            //schedule.StartTime = TimeOnly.Parse(request.StartTime);
            //schedule.EndTime = TimeOnly.Parse(request.EndTime);

            var result = await _repo.CreateAsync(schedule);

			if (result == null)
				return Result<Guid>.Failure(new[] { "Create schedule failed" });

            return Result<Guid>.Success(result.Id);
		}
    }
}
