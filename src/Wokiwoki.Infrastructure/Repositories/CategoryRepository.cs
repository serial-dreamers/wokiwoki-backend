using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Common.Interfaces.Repositories;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class CategoryRepository : BaseRepo<Category, Guid>, ICategoryRepository
	{
		public CategoryRepository(WokiwokiDbContext context) : base(context)
		{
		}

		public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken)
		{
		 return await _context.Categories.Where(c => c.IsActive).ToListAsync(cancellationToken);
		}
	}
}
