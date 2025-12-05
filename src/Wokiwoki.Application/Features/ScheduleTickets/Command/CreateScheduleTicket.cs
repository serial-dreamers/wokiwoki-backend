using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.ScheduleTickets.Command
{
    //public sealed record CreateScheduleTicketCommand(
    //     Guid WorkshopScheduleId ,
    //     string Name,
    //     decimal Price ,
    //     int MaxQuantity
    //    ) : IRequest<WorkshopScheduleTicket>;

    public sealed record CreateScheduleTicketCommand : IRequest<Result<Guid>>
    {
        public Guid WorkshopScheduleId { get; init; }
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public int MaxQuantity { get; init; }
    }
	public class CreateScheduleTicketCommandHandler : IRequestHandler<CreateScheduleTicketCommand, Result<Guid>>
	{
		private readonly IWorkshopScheduleTicketRepository _ticketRepo;
		private readonly IWorkshopScheduleRepository _scheduleRepo;
		private readonly IMapper _mapper;
		private readonly IUuidService _uuidService;

		public CreateScheduleTicketCommandHandler(
			IWorkshopScheduleTicketRepository ticketRepo,
			IWorkshopScheduleRepository scheduleRepo,
			IMapper mapper,
			IUuidService uuidService)
		{
			_ticketRepo = ticketRepo;
			_scheduleRepo = scheduleRepo;
			_mapper = mapper;
			_uuidService = uuidService;
		}

		public async Task<Result<Guid>> Handle(CreateScheduleTicketCommand request, CancellationToken cancellationToken)
		{
			// 1️⃣ Kiểm tra xem Schedule có tồn tại không
			var schedule = await _scheduleRepo.GetByIdAsync(request.WorkshopScheduleId);
			if (schedule == null)
				return Result<Guid>.Failure(new[] { "Workshop schedule not found" });
			 
			var ticket = _mapper.Map<WorkshopScheduleTicket>(request);
			ticket.Id = _uuidService.NewGuid();
			ticket.IsActive = true;
			ticket.Created = DateTime.UtcNow;
			 
			var result = await _ticketRepo.CreateAsync(ticket, cancellationToken);
			 
			if (result == null)
				return Result<Guid>.Failure(new[] { "Failed to create schedule ticket" });

			return Result<Guid>.Success(result.Id);
		}
	}
}
