using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Common.Interfaces.Repositories;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class CategoryRepository : BaseRepo<Category, Guid>, ICategoryRepository
	{
		public CategoryRepository(WokiwokiDbContext context) : base(context)
		{
		}

		public async Task<List<Category>> GetAllActiveAsync(CancellationToken cancellationToken)
		=> await _context.Categories.Where(c => c.IsActive).ToListAsync(cancellationToken);
		

		public async Task<List<Category>> GetAllCategoriesWithTagsAsync(CancellationToken cancellationToken)
		=> await _context.Categories
				.Include(c => c.Tags)
				.Where(c => c.IsActive)
				.ToListAsync(cancellationToken);
		
	}
}
