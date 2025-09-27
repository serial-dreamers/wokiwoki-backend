using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
	public interface ICategoryRepository : IBaseRepo<Category, Guid>
	{
		Task<List<Category>> GetAllAsync(CancellationToken cancellationToken);
	}
}
