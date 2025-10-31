using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.ScheduleTickets.Command
{
    public sealed record CreateScheduleTicketCommand(
         Guid WorkshopScheduleId ,
         string Name,
         decimal Price ,
         int MaxQuantity
        ) : IRequest<WorkshopScheduleTicket>;

    public class CreateScheduleTicket : IRequestHandler<CreateScheduleTicketCommand, WorkshopScheduleTicket>
    {
        private readonly IWorkshopScheduleTicketRepository _workshopScheduleTicketRepository;
        private readonly IMapper _mapper;
        public CreateScheduleTicket(IWorkshopScheduleTicketRepository workshopScheduleTicketRepository, IMapper mapper)
        {
            _workshopScheduleTicketRepository = workshopScheduleTicketRepository;
            _mapper = mapper;
        }

        public async Task<WorkshopScheduleTicket> Handle(CreateScheduleTicketCommand request, CancellationToken cancellationToken)
        {
            var entity = new WorkshopScheduleTicket();
            _mapper.Map(request, entity);
            var result = await _workshopScheduleTicketRepository.CreateAsync(entity, cancellationToken);
            return result;
        }
    }
}
