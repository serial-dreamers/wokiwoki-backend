using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Common.Interfaces.Repositories
{
	public interface IOrganizationRepository : IBaseRepo<Organization, Guid>
	{
		Task<Guid?> GetOrganizationIdByUserIdAsync(string userId);

		Task<List<Organization>> GetOrganizationsByIdsAsync(List<Guid> organizationIds, CancellationToken cancellationToken = default);

		Task IncrementFollowerCountAsync(Guid organizationId, CancellationToken cancellationToken);

		Task DecrementFollowerCountAsync(Guid organizationId, CancellationToken cancellationToken);

		Task<List<Organization>> GetTopOrganizationsByFollowerCountAsync(int limit, CancellationToken cancellationToken = default);

		Task<Organization?> GetOrganizationByOwnerIdAsync(string ownerId, CancellationToken cancellationToken = default);

		Task<List<WorkshopSimpleDto>> GetOrganizerWorkshopsAsync(string userId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get admin organizations list with filters
		/// </summary>
		Task<PaginatedList<AdminOrganizationDto>> GetAdminOrganizationsAsync(
			int? status,
			string? searchTerm,
			int pageNumber,
			int pageSize,
			CancellationToken cancellationToken = default);
	}
}
