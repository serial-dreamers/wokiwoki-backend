using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Features.Tags.Queries;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class TagRepository : BaseRepo<Tag, Guid>, ITagRepository
	{
		public TagRepository(WokiwokiDbContext context) : base(context)
		{
		}

		public async Task<List<Tag>> GetTagsByCategory(Guid categoryId, CancellationToken cancellationToken)
		{
			return await _context.Tags
				.Where(t => t.IsActive == true && t.Categories.Any(c => c.Id == categoryId)) 
				.ToListAsync(cancellationToken); 
		}

		public async Task<List<Tag>> GetTagsByIdsAsync(List<Guid> ids)
		{
			return await _context.Tags
				.Where(t => ids.Contains(t.Id) && t.IsActive == true)
				.ToListAsync();
		}
	}
}
