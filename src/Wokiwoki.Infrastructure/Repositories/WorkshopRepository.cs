using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.Features.Workshops.Queries.SearchWorkshop;
using Wokiwoki.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class WorkshopRepository : BaseRepo<Workshop, Guid>, IWorkshopRepository
	{
		public WorkshopRepository(WokiwokiDbContext context) : base(context)
		{

		}

		public async Task<List<Workshop>> SearchAsync(
	 SearchWorkshopQuery request,
	 CancellationToken cancellationToken = default)
		{
			var query = _context.Workshops
				.Include(w => w.WorkshopSessions)
				.ThenInclude(w => w.WorkshopTicketTypes)
				.Include(w => w.Tags)
				.Include(w => w.Category)
				.Include(w => w.WorkshopType)
				.AsQueryable();

			var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
			var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

			var L = await query.Where(w =>
				// category filter
				(!request.cateId.HasValue || w.CategoryId == request.cateId.Value)

				// tag filter
				|| (request.tagIdList == null || !request.tagIdList.Any()
					|| w.Tags.Any(t => request.tagIdList.Contains(t.Id)))

				// typeDate filter
				|| (!string.IsNullOrEmpty(request.typeDate) &&
					((request.typeDate.ToLower() == "today" && w.StartTime.Date == vietnamNow.Date)
					 || (request.typeDate.ToLower() == "upcoming" && w.StartTime > vietnamNow)))

				// date range filter
				|| (request.startDate.HasValue && request.endDate.HasValue &&
					w.StartTime >= request.startDate.Value && w.EndTime <= request.endDate.Value)

				// free filter
				|| (request.isFree.HasValue &&
					(request.isFree.Value
						? w.WorkshopSessions.All(s => s.WorkshopTicketTypes.All(t => t.Price == 0))
						: w.WorkshopSessions.Any(s => s.WorkshopTicketTypes.Any(t => t.Price > 0))
					))

				// type filter
				|| (!request.typeId.HasValue || w.WorkshopTypeId == request.typeId.Value)
			).ToListAsync();
			//var wL = await query.ToPaginatedListAsync(request.pageNo, request.pageSize, cancellationToken);
			return L;
		}
		public async Task<List<Workshop>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			var wL = await _context.Workshops
				.Include(w => w.WorkshopSessions)
					.ThenInclude(s => s.WorkshopTicketTypes)
				.Include(w => w.Tags)
				.Include(w => w.Category)
				.Include(w => w.WorkshopType)
				.ToListAsync(cancellationToken);

			return wL;
		}
		public async Task<List<Workshop>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var w = await _context.Workshops
				.Include(w => w.WorkshopSessions)
					.ThenInclude(s => s.WorkshopTicketTypes)
				.Include(w => w.Tags)
				.Include(w => w.Category)
				.Include(w => w.WorkshopType)
				.Where(w => w.Id == id)
				.ToListAsync(cancellationToken);

			return w;
		}
		//public async Task<bool> UpdateAsync(Guid id, CancellationToken cancellationToken = default)
		//{
		//	var w = await _context.Workshops.FirstOrDefaultAsync(w => w.Id == id);
		//	w.
		//}

	}
}
