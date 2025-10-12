using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Features.Workshops.Queries.GetFilterPagedWorkshopsQuery;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class WorkshopRepository : BaseRepo<Workshop, Guid>, IWorkshopRepository
	{
		public WorkshopRepository(WokiwokiDbContext context) : base(context)
		{
		}

		public async Task<PaginatedList<Workshop>> SearchAsync(
	 SearchWorkshopQuery request,
	 CancellationToken cancellationToken = default)
		{
			var query = _context.Workshops
				.Include(w => w.WorkshopSessions)
				.ThenInclude(s => s.WorkshopTicketTypes)
				.Include(w => w.Tags)
				.Include(w => w.Category)
				.Include(w => w.WorkshopType)
				.AsQueryable();

			var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
			var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
			// category
			if (request.cateId.HasValue)
				query = query.Where(w => w.CategoryId == request.cateId.Value);

			// tag
			if (request.tagIdList != null && request.tagIdList.Any())
				query = query.Where(w => w.Tags.Any(t => request.tagIdList.Contains(t.Id)));

			// date type
			if (!string.IsNullOrEmpty(request.typeDate))
			{
				if (request.typeDate.Equals("today", StringComparison.OrdinalIgnoreCase))
					query = query.Where(w => w.StartTime.Date == vietnamNow.Date);
				else if (request.typeDate.Equals("upcoming", StringComparison.OrdinalIgnoreCase))
					query = query.Where(w => w.StartTime > vietnamNow);
			}

			// date range
			if (request.startDate.HasValue && request.endDate.HasValue)
				query = query.Where(w => w.StartTime >= request.startDate && w.EndTime <= request.endDate);

			// free
			if (request.isFree.HasValue)
			{
				if (request.isFree.Value)
					query = query.Where(w => w.WorkshopSessions.All(s => s.WorkshopTicketTypes.All(t => t.Price == 0)));
				else
					query = query.Where(w => w.WorkshopSessions.Any(s => s.WorkshopTicketTypes.Any(t => t.Price > 0)));
			}
			 
			if (request.typeId.HasValue)
				query = query.Where(w => w.WorkshopTypeId == request.typeId.Value);

			var totalCount = await query.CountAsync(cancellationToken);



			var listWorkshop = await query.OrderByDescending(w => w.StartTime)
							.ToPaginatedListAsync(request.pageNo, request.pageSize, cancellationToken);

			return listWorkshop;
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
			var workshop = await _context.Workshops
				.Include(w => w.WorkshopSessions)
					.ThenInclude(s => s.WorkshopTicketTypes)
				.Include(w => w.Tags)
				.Include(w => w.Category)
				.Include(w => w.WorkshopType)
				.Where(w => w.Id == id)
				.ToListAsync(cancellationToken);

			return workshop;
		}
		//public async Task<bool> UpdateAsync(Guid id, CancellationToken cancellationToken = default)
		//{
		//	var w = await _context.Workshops.FirstOrDefaultAsync(w => w.Id == id);
		//	w.
		//}

	}
}
