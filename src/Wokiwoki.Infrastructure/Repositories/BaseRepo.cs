using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Common.Interfaces.Repositories;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class BaseRepo<T, Tkey> : IBaseRepo<T, Tkey> where T : class
	{
		protected readonly WokiwokiDbContext _context;

		public BaseRepo()
		{
			_context ??= new WokiwokiDbContext();
		}
		public BaseRepo(WokiwokiDbContext context)
		{
			_context = context;
		}
		public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
		{
			_context.Set<T>().Add(entity);
			await _context.SaveChangesAsync(cancellationToken);
			return entity;
		}

		public async Task<bool> DeleteAsync(Tkey id, CancellationToken cancellationToken = default)
		{
			var entity = await _context.Set<T>().FindAsync(new object[] { id! }, cancellationToken);
			if (entity == null) return false;

			_context.Set<T>().Remove(entity);
			await _context.SaveChangesAsync(cancellationToken);
			return true;
		}


		public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await _context.Set<T>().ToListAsync(cancellationToken);
		}

		public async Task<T?> GetByIdAsync(Tkey id, CancellationToken cancellationToken = default)
		{
			return await _context.Set<T>().FindAsync(new object[] { id! }, cancellationToken);
		}

		public async Task<bool> SaveChangeAsync(CancellationToken cancellationToken = default)
		{
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> UpdateAsync(Tkey id, T entity, CancellationToken cancellationToken = default)
		{
			var existing = await _context.Set<T>().FindAsync(new object[] { id! }, cancellationToken);
			if (existing == null) return false;

			_context.Entry(existing).CurrentValues.SetValues(entity);
			await _context.SaveChangesAsync(cancellationToken);
			return true;
		}

		public async Task<T?> UpdateTAsync(Tkey id, T entity, CancellationToken cancellationToken = default)
		{
			var existing = await _context.Set<T>().FindAsync(new object[] { id! }, cancellationToken);
			if (existing == null) return null;

			_context.Entry(existing).CurrentValues.SetValues(entity);
			await _context.SaveChangesAsync(cancellationToken);
			return existing;
		}

		/// <summary>
		/// Lấy entity theo Id, chỉ những entity có IsActive = true
		/// </summary>
		public async Task<T?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var property = typeof(T).GetProperty("Id");
			var isActiveProperty = typeof(T).GetProperty("IsActive");

			if (property == null || isActiveProperty == null)
				throw new InvalidOperationException("Entity must have Id and IsActive properties");

			return await _context.Set<T>()
				.AsNoTracking()
				.FirstOrDefaultAsync(e =>
					(Guid)property.GetValue(e)! == id &&
					(bool)isActiveProperty.GetValue(e)!,
					cancellationToken);
		}
	}
}
