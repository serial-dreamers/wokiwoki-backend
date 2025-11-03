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

        Task<List<Workshop>> GetAllActiveAsync(CancellationToken cancellationToken = default);

        Task<List<Workshop>> GetByIdActiveAsync(Guid id, CancellationToken cancellationToken = default);

		Task IncrementLikeCountAsync(Guid workshopId, CancellationToken cancellationToken);
		Task DecrementLikeCountAsync(Guid workshopId, CancellationToken cancellationToken);

        Task<bool> CheckWorkshopExistById(Guid workshopId, CancellationToken cancellationToken);

        Task<Workshop?> GetWorkshopById(Guid workshopId, CancellationToken cancellationToken);

        Task<PaginatedList<Workshop>> GetWorkshopByWorkshopIdAsync(Guid orgId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
	}
}
