using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.ScheduleTickets.Command
{
	public sealed record UpdateScheduleTicketCommand : IRequest<Result<Guid>>
	{
		public Guid Id { get; init; }
		public string Name { get; init; } = string.Empty;
		public decimal Price { get; init; }
		public int MaxQuantity { get; init; }
		public bool IsActive { get; init; }
	}

	public class UpdateScheduleTicketCommandHandler : IRequestHandler<UpdateScheduleTicketCommand, Result<Guid>>
	{
		private readonly IWorkshopScheduleTicketRepository _ticketRepo;
		private readonly IMapper _mapper;

		public UpdateScheduleTicketCommandHandler(
			IWorkshopScheduleTicketRepository ticketRepo,
			IMapper mapper)
		{
			_ticketRepo = ticketRepo;
			_mapper = mapper;
		}

		public async Task<Result<Guid>> Handle(UpdateScheduleTicketCommand request, CancellationToken cancellationToken)
		{
			// 1️⃣ Kiểm tra xem ticket có tồn tại không
			var existingTicket = await _ticketRepo.GetByIdAsync(request.Id);
			if (existingTicket == null)
				return Result<Guid>.Failure(new[] { "Schedule ticket not found" });

			// 2️⃣ Cập nhật thông tin
			existingTicket.Name = request.Name;
			existingTicket.Price = request.Price;
			existingTicket.MaxQuantity = request.MaxQuantity;
			existingTicket.IsActive = request.IsActive;
			existingTicket.LastModified = DateTime.UtcNow;

			// 3️⃣ Lưu vào DB
			var result = await _ticketRepo.UpdateTAsync(existingTicket.Id,existingTicket, cancellationToken);

			if (result == null)
				return Result<Guid>.Failure(new[] { "Failed to update schedule ticket" });

			return Result<Guid>.Success(result.Id);
		}
	}
}

