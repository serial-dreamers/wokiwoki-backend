using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
    public interface IWorkshopSessionRepository : IBaseRepo<WorkshopSession, Guid>
    {
        Task<List<WorkshopSession>> Create1MonthSession(Guid scheduleId, WorkshopSession session, CancellationToken cancellationToken = default);

        Task<List<WorkshopSession>> GetSessionByScheduleId(Guid scheduleId, CancellationToken cancellationToken = default);

		Task<List<WorkshopSession>> GetSessionsWeekByScheduleId(
				Guid scheduleId,
				DateTime? startDate,
				DateTime? endDate,
				CancellationToken cancellationToken);

	}
}
