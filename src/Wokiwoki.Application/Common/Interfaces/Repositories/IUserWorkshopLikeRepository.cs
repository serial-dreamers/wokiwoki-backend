using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
	public interface IUserWorkshopLikeRepository : IBaseRepo<UserWorkshopLike, Guid>
	{
		Task<bool> ExistsAsync(string userId, Guid workshopId, CancellationToken cancellationToken);

		Task<UserWorkshopLike> GetExistsAsync(string userId, Guid workshopId, CancellationToken cancellationToken);

	}
}
