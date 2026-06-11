using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Tickets.Queries.GetOrganizerParticipants
{
	public class GetOrganizerParticipantsQueryHandler : IRequestHandler<GetOrganizerParticipantsQuery, PaginatedList<ParticipantDto>>
	{
		private readonly ITicketRepository _ticketRepository;
		private readonly IUserContext _userContext;

		public GetOrganizerParticipantsQueryHandler(
			ITicketRepository ticketRepository,
			IUserContext userContext)
		{
			_ticketRepository = ticketRepository;
			_userContext = userContext;
		}

		public async Task<PaginatedList<ParticipantDto>> Handle(GetOrganizerParticipantsQuery request, CancellationToken cancellationToken)
		{
			// Get current user ID
			var userId = _userContext.UserId;
			if (string.IsNullOrEmpty(userId))
				throw new UnauthorizedAccessException("User not authenticated");

		// Use repository method to get participants
		return await _ticketRepository.GetOrganizerParticipantsAsync(
			userId,
			request.WorkshopId,
			request.SessionId,
			request.StartDate,
			request.EndDate,
			request.IsCheckedIn,
			request.SearchTerm,
			request.PageNumber,
			request.PageSize,
			cancellationToken
		);
		}
	}
}

