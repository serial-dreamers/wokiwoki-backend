using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
	public interface IWorkshopMediaRepository : IBaseRepo<WorkshopMedia, Guid>
	{
		Task<List<WorkshopMedia>> GetActiveMediaByWorkshopIdAsync(Guid workshopId);
	}
}
