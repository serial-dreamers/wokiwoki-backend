using Wokiwoki.Domain.Entities;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
	public interface ITicketRepository : IBaseRepo<Ticket, Guid>
	{
		Task<Ticket?> GetTicketWithDetailsAsync(Guid ticketId, CancellationToken cancellationToken = default);
		Task<Result> CheckInTicketAsync(Guid ticketId, CancellationToken cancellationToken = default);
	Task<PaginatedList<ParticipantDto>> GetOrganizerParticipantsAsync(
		string userId,
		Guid? workshopId,
		Guid? sessionId,
		DateTime? startDate,
		DateTime? endDate,
		bool? isCheckedIn,
		string? searchTerm,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default);
	}
}
