using MediatR;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Tickets.Queries.GetOrganizerParticipants
{
	/// <summary>
	/// Query to get participants (tickets) for organizer's workshops with filtering and paging
	/// </summary>
	public record GetOrganizerParticipantsQuery(
		Guid? WorkshopId = null,
		Guid? SessionId = null,
		DateTime? StartDate = null,
		DateTime? EndDate = null,
		bool? IsCheckedIn = null,
		string? SearchTerm = null, // Search by fullName or phoneNumber
		int PageNumber = 1,
		int PageSize = 20
	) : IRequest<PaginatedList<ParticipantDto>>;
}

