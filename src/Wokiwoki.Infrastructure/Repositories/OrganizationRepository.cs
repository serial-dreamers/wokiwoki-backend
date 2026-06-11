using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response; 
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class OrganizationRepository : BaseRepo<Organization, Guid>, IOrganizationRepository
	{
		public OrganizationRepository(WokiwokiDbContext context) : base(context)
		{
		} 

		public async Task<Guid?> GetOrganizationIdByUserIdAsync(string userId)
		{
			var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.OwnerId == userId);
			return organization?.Id;
		}

		public async Task<List<Organization>> GetOrganizationsByIdsAsync(List<Guid> organizationIds, CancellationToken cancellationToken = default)
		{
			return await _context.Organizations
				.Where(org => organizationIds.Contains(org.Id)
					&& org.IsActive
					&& org.Status == OrganizationStatus.Accepted)
				.ToListAsync(cancellationToken);
		}

		public async Task IncrementFollowerCountAsync(Guid organizationId, CancellationToken cancellationToken)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync(
				$"UPDATE organization SET followercount = followercount + 1 WHERE id = {organizationId}",
				cancellationToken
			);
		}


		public async Task DecrementFollowerCountAsync(Guid organizationId, CancellationToken cancellationToken)
		{
			await _context.Database.ExecuteSqlInterpolatedAsync(
		$"UPDATE organization SET followercount = CASE WHEN followercount > 0 THEN followercount - 1 ELSE 0 END WHERE id = {organizationId}",
			cancellationToken
			);
		}

		public async Task<List<Organization>> GetTopOrganizationsByFollowerCountAsync(int limit, CancellationToken cancellationToken = default)
		{
			return await _context.Organizations
				.Where(org => org.IsActive 
					&& org.Status == OrganizationStatus.Accepted 
					&& org.FollowerCount > 0)
				.OrderByDescending(org => org.FollowerCount)
				.Take(limit)
				.ToListAsync(cancellationToken);
		}

		public async Task<Organization?> GetOrganizationByOwnerIdAsync(string ownerId, CancellationToken cancellationToken = default)
		{
			return await _context.Organizations
				.FirstOrDefaultAsync(o => o.OwnerId == ownerId, cancellationToken);
		}

		public async Task<List<WorkshopSimpleDto>> GetOrganizerWorkshopsAsync(string userId, CancellationToken cancellationToken = default)
		{
			var organizationIds = await _context.Organizations
				.Where(o => o.OwnerId == userId)
				.Select(o => o.Id)
				.ToListAsync(cancellationToken);

			if (!organizationIds.Any())
				return new List<WorkshopSimpleDto>();

			return await _context.Workshops
				.Where(w => organizationIds.Contains(w.OrganizationId))
				.Where(w => w.IsActive)
				.OrderByDescending(w => w.Created)
				.Select(w => new WorkshopSimpleDto
				{
					Id = w.Id,
					Title = w.Title,
					ImageUrl = w.ImageUrl
				})
				.ToListAsync(cancellationToken);
		}

		public async Task<PaginatedList<AdminOrganizationDto>> GetAdminOrganizationsAsync(
			int? status,
			string? searchTerm,
			int pageNumber,
			int pageSize,
			CancellationToken cancellationToken = default)
		{
			var query = _context.Organizations
				.AsNoTracking()
				.AsQueryable();

			// Filter by status
			if (status.HasValue)
			{
				query = query.Where(o => (int)o.Status == status.Value);
			}

			// Filter by search term
			if (!string.IsNullOrWhiteSpace(searchTerm))
			{
				var searchLower = searchTerm.ToLower();
				query = query.Where(o =>
					o.Name.ToLower().Contains(searchLower) ||
					(o.ContactEmail != null && o.ContactEmail.ToLower().Contains(searchLower)));
			}

			// Get total count
			var totalCount = await query.CountAsync(cancellationToken);

			// Apply pagination
			var organizations = await query
				.OrderByDescending(o => o.Created)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync(cancellationToken);

			var dtos = new List<AdminOrganizationDto>();
			foreach (var org in organizations)
			{
				var totalWorkshops = await _context.Workshops
					.Where(w => w.OrganizationId == org.Id && w.IsActive)
					.CountAsync(cancellationToken);

				var owner = await _context.Users.FindAsync(new object[] { org.OwnerId }, cancellationToken);

				dtos.Add(new AdminOrganizationDto
				{
					Id = org.Id,
					Name = org.Name,
					Description = org.Description,
					LogoUrl = org.LogoUrl,
					ContactEmail = org.ContactEmail,
					ContactPhone = org.ContactPhone,
					Address = string.Join(", ", new[] { org.Street, org.Commune, org.Province }.Where(s => !string.IsNullOrEmpty(s))),
					FollowerCount = org.FollowerCount,
					Status = (int)org.Status,
					Reason = null, // Can add Reason field to Organization entity if needed
					OwnerName = owner?.FullName ?? "Unknown",
					OwnerEmail = owner?.Email ?? "Unknown",
					TotalWorkshops = totalWorkshops,
					Created = org.Created
				});
			}

			return new PaginatedList<AdminOrganizationDto>(dtos, totalCount, pageNumber, pageSize);
		}
	}
}
