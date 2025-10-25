using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Workshops.Queries.GetWorkshop;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.WorkshopSchedules.Queries.GetSchedule
{
    public sealed record GetScheduleByIdQuery(Guid Id) : IRequest<WorkshopSchedule>;
    public class GetScheduleByIdQueryHandler : IRequestHandler<GetScheduleByIdQuery, WorkshopSchedule?>
    {
        //private readonly IWorkshopRepository _workshopRepository;
        private readonly IWorkshopScheduleRepository _repo;
        private readonly IMapper _mapper;

        public async Task<WorkshopSchedule?> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
        {
            var s = await _repo.GetByIdAsync(request.Id);
            if (s == null)
            {
                return null;
            }
            return s;
        }
    }
}
