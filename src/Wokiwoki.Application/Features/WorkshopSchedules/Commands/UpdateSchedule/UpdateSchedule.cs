using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.WorkshopSchedules.Commands.UpdateSchedule
{
    public sealed record UpdateScheduleCommand(
         Guid Id,

         RecurrenceType RecurrenceType,

         string? DaysOfWeek,
         string? DaysOfMonth,

         TimeOnly StartTime,
         TimeOnly EndTime,

         DateTime ValidFrom,
         DateTime? ValidUntil,

     int? Capacity
        ) : IRequest<WorkshopSchedule>;

    public class UpdateSchedule : IRequestHandler<UpdateScheduleCommand, WorkshopSchedule>
    {
        private readonly IWorkshopScheduleRepository _repo;
        private readonly IMapper _mapper;
        public UpdateSchedule(IWorkshopScheduleRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<WorkshopSchedule> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
        {
            var exist = await _repo.GetByIdAsync(request.Id);
            if (exist == null)
            {
                return null;
            }
            _mapper.Map(request, exist);
            var result = await _repo.UpdateTAsync(exist.Id, exist, cancellationToken);
            return result;
        }
    }
}
