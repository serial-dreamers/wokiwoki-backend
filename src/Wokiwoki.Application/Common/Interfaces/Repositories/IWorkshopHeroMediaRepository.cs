using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.WorkshopHeroMedias;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
	public interface IWorkshopHeroMediaRepository : IBaseRepo<WorkshopHeroMedia, Guid>
	{
		Task SyncHeroMediaAsync(
		Guid workshopId,
		List<WorkshopHeroMediaDto> heroMediaDtos,
		CancellationToken cancellationToken = default);

		Task<List<WorkshopHeroMedia>> GetHeroMediasByWorkshopId(Guid workshopId);
	}
}
