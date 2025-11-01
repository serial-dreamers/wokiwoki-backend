using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.ScheduleTickets.Command
{
    //public sealed record CreateScheduleTicketCommand(
    //     Guid WorkshopScheduleId ,
    //     string Name,
    //     decimal Price ,
    //     int MaxQuantity
    //    ) : IRequest<WorkshopScheduleTicket>;

    public sealed record CreateScheduleTicketCommand : IRequest<WorkshopScheduleTicket>
    {
        public Guid WorkshopScheduleId { get; init; }
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public int MaxQuantity { get; init; }
    }
    public class CreateScheduleTicket : IRequestHandler<CreateScheduleTicketCommand, WorkshopScheduleTicket>
    {
        private readonly IWorkshopScheduleTicketRepository _workshopScheduleTicketRepository;
        private readonly IUuidService _uuidService;
        private readonly IMapper _mapper;
        public CreateScheduleTicket(IWorkshopScheduleTicketRepository workshopScheduleTicketRepository,
            IUuidService uuidService,
            IMapper mapper)
        {
            _workshopScheduleTicketRepository = workshopScheduleTicketRepository;
            _uuidService = uuidService;
            _mapper = mapper;
        }

        public async Task<WorkshopScheduleTicket> Handle(CreateScheduleTicketCommand request, CancellationToken cancellationToken)
        {
            var entity = new WorkshopScheduleTicket();
            _mapper.Map(request, entity);
            entity.Id = _uuidService.NewGuid();
            entity.IsActive = true;
            var result = await _workshopScheduleTicketRepository.CreateAsync(entity, cancellationToken);
            return result;
        }
    }
}
