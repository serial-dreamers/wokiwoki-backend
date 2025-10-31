using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
	public interface IBaseRepo<T, Tkey> where T : class
	{
		Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
		Task<bool> DeleteAsync(Tkey id, CancellationToken cancellationToken = default);
		Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
		Task<T?> GetByIdAsync(Tkey id, CancellationToken cancellationToken = default);
		Task<bool> SaveChangeAsync(CancellationToken cancellationToken = default);
		Task<bool> UpdateAsync(Tkey id, T entity, CancellationToken cancellationToken = default);
		Task<T?> UpdateTAsync(Tkey id, T entity, CancellationToken cancellationToken = default);

		Task<T?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken = default);

	}
}
