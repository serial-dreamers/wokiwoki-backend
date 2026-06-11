using MediatR;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.WorkshopSessions.Queries.GetSessionsByWorkshop
{
	/// <summary>
	/// Query to get sessions for a specific workshop (for dropdown/filter)
	/// </summary>
	public record GetSessionsByWorkshopQuery(Guid WorkshopId) : IRequest<List<SessionSimpleDto>>;
}

