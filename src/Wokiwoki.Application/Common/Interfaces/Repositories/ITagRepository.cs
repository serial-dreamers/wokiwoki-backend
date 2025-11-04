using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
	public interface ITagRepository : IBaseRepo<Tag, Guid>
	{
		Task<List<Tag>> GetTagsByIdsAsync(List<Guid> ids);

		Task<List<Tag>> GetTagsByCategory(Guid categoryId, CancellationToken cancellationToken);
	}
}
