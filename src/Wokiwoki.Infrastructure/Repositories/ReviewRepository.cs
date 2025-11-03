using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Infrastructure.Data.Extensions;

namespace Wokiwoki.Infrastructure.Repositories
{
    public class ReviewRepository : BaseRepo<Review, Guid>, IReviewRepository
    {
        public ReviewRepository(WokiwokiDbContext context) : base(context) { }

        public async Task<PaginatedList<Review>> GetByWorkshopId(Guid workshopId, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            return await _context.Reviews.Where(r => r.WorkshopId == workshopId).OrderByDescending(r => r.Created).ToPaginatedListAsync(pageNo, pageSize, cancellationToken);
        }
    }
}
