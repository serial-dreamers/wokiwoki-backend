using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Workshops.Queries.GetFilterPagedWorkshopsQuery;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

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

		Task<PaginatedList<Workshop>> GetByOrganizationWithFilterAsync(
	        Guid organizationId,
	        string? title,
			WorkshopStatus? status,
	        int pageNumber,
	        int pageSize,
	        CancellationToken cancellationToken = default
        );

		/// <summary>
		/// Lấy danh sách WorkshopSessions theo OrganizationId và date range (cho Calendar view)
		/// </summary>
		Task<List<WorkshopSession>> GetSessionsByOrganizationAndDateRangeAsync(
			Guid organizationId,
			DateTime startDate,
			DateTime endDate,
			CancellationToken cancellationToken = default
		);

		/// <summary>
		/// Lấy danh sách Organization IDs có workshops trong category cụ thể
		/// </summary>
		Task<List<Guid>> GetOrganizationIdsByCategoryAsync(Guid categoryId, int limit, CancellationToken cancellationToken = default);

		/// <summary>
		/// Lấy workshops theo tag IDs (dành cho personalized recommendations)
		/// </summary>
		Task<List<Workshop>> GetWorkshopsByTagIdsAsync(List<Guid> tagIds, int limit, CancellationToken cancellationToken = default);

		/// <summary>
		/// Lấy workshops có sessions trong khoảng thời gian cụ thể
		/// </summary>
		Task<List<Workshop>> GetWorkshopsByDateRangeAsync(DateTime startDate, DateTime endDate, int limit, CancellationToken cancellationToken = default);

		Task<List<Workshop>> GetDiscoverWorkshopsAsync(int limit, CancellationToken cancellationToken = default);
	}
}
