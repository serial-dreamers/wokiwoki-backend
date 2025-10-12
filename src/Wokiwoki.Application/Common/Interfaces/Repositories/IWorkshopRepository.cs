using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.Features.Workshops.Queries.GetFilterPagedWorkshopsQuery;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
    public interface IWorkshopRepository : IBaseRepo<Workshop, Guid>
    {
        //Task<PaginatedList<Workshop>> GetAll(int pageNo = 1, int pageSize = 3, CancellationToken cancellationToken = default);
        Task<PaginatedList<Workshop>> SearchAsync(
            SearchWorkshopQuery request,
            CancellationToken cancellationToken = default);
        Task<List<Workshop>> GetAllAsync(CancellationToken cancellationToken = default);

	}
}
