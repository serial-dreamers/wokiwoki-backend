using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Repositories;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class TagRepository : BaseRepo<Tag, Guid>, ITagRepository
	{
		public TagRepository(WokiwokiDbContext context) : base(context)
		{
		}

		public async Task<List<Tag>> GetTagsByIdsAsync(List<Guid> ids)
		{
			return await _context.Tags
				.Where(t => ids.Contains(t.Id) && t.IsActive == true)
				.ToListAsync();
		}
	}
}
