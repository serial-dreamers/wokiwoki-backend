using MediatR; 
using System.Security.Claims;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.Features.Tickets.Commands.CheckInTicket;

namespace Wokiwoki.Application.Features.Tickets.Commands.CheckInTicket
{
	public record CheckInTicketCommand(Guid TicketId) : IRequest<Result>;

	public class CheckInTicketCommandHandler : IRequestHandler<CheckInTicketCommand, Result>
	{
		private readonly ITicketRepository _ticketRepository;
		private readonly IUserContext _userContext;

		public CheckInTicketCommandHandler(
			ITicketRepository ticketRepository,
			IUserContext userContext)
		{
			_ticketRepository = ticketRepository;
			_userContext = userContext;
		}

		public async Task<Result> Handle(CheckInTicketCommand request, CancellationToken cancellationToken)
		{
			// Get current user ID
			var userId = _userContext.UserId;
			if (string.IsNullOrEmpty(userId))
				return Result.Failure(new[] { "User not authenticated" });

			// Get ticket with related data to check authorization
			var ticket = await _ticketRepository.GetTicketWithDetailsAsync(request.TicketId, cancellationToken);

			if (ticket == null)
				return Result.Failure(new[] { "Ticket not found" });
			 
			if (ticket.Booking.Workshop.Organization.OwnerId != userId)
				return Result.Failure(new[] { "You are not authorized to check-in this ticket" });

			// Use repository method to handle check-in
			return await _ticketRepository.CheckInTicketAsync(request.TicketId, cancellationToken);
		}
	}
}
