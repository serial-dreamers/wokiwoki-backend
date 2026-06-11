using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
    public interface IReviewRepository : IBaseRepo<Review, Guid>
    {
        Task<PaginatedList<Review>> GetByWorkshopId(Guid workshopId, int pageNo, int pageSize, CancellationToken cancellationToken);

        Task<Review?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken);
	}
}
