using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Common.Interfaces.Repositories;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class WorkshopMediaRepository : BaseRepo<WorkshopMedia, Guid>, IWorkshopMediaRepository
	{
		public WorkshopMediaRepository(WokiwokiDbContext context) : base(context)
		{
		}

		public async Task<List<WorkshopMedia>> GetActiveMediaByWorkshopIdAsync(Guid workshopId)
		{
			return await _context.WorkshopMedias
				.Where(wm => wm.WorkshopId == workshopId && wm.IsActive)
				.ToListAsync();
		}
	} 
}
