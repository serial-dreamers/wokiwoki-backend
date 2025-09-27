using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
	public interface ITagRepository : IBaseRepo<Tag, Guid>
	{
		Task<List<Tag>> GetTagsByIdsAsync(List<Guid> ids);
	}
}
