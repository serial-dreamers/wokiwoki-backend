using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Repositories; 
using Wokiwoki.Infrastructure.Data.Extensions;

namespace Wokiwoki.Infrastructure.Repositories
{
    public class WorkshopRepository : BaseRepo<Workshop, Guid>, IWorkshopRepository
    {
        public WorkshopRepository(WokiwokiDbContext context) : base(context)
        {

        }
    //    public async Task<PaginatedList<Workshop>> Search(
    //SearchWorkshopQuery request,
    //int page,
    //int pageSize,
    //CancellationToken cancellationToken = default)
    //    {
    //        var query = _context.Workshops.AsQueryable();

    //        query = query.Where(o =>
    //            // category filter
    //            (!request.cateId.HasValue || o.CategoryId == request.cateId.Value)

    //            // tag filter
    //            || (request.tagIdList == null || request.tagIdList.Count == 0
    //                || o.Tags.Any(t => request.tagIdList.Contains(t.Id)))

    //            // typeDate filter
    //            || (!string.IsNullOrEmpty(request.typeDate) &&
    //                ((request.typeDate.ToLower() == "today" && o.StartTime.Date == DateTime.Today) ||
    //                 (request.typeDate.ToLower() == "upcoming" && o.StartTime > DateTime.Now)))

    //            // date range filter
    //            || (request.startDate.HasValue && request.endDate.HasValue &&
    //                o.StartTime >= request.startDate.Value && o.EndTime <= request.endDate.Value)

    //            // free filter (dựa trên ticket type)
    //            || (request.isFree.HasValue && (
    //                request.isFree.Value
    //                    ? o.WorkshopSessions.All(s => s.WorkshopTicketTypes.All(t => t.Price == 0)) // tất cả vé free
    //                    : o.WorkshopSessions.Any(s => s.WorkshopTicketTypes.Any(t => t.Price > 0))  // có ít nhất 1 vé có giá
    //            ))

    //            // type filter
    //            || (!request.typeId.HasValue || o. == request.typeId.Value)
    //        );

    //        return await query.ToPaginatedListAsync(page, pageSize, cancellationToken);
    //    }



    }
}
