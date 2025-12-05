using AutoMapper;
using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Organizations.Queries.GetOrganizationsByCategory
{
	public record GetOrganizationsByCategoryQuery(List<Guid> CategoryIds, int LimitPerCategory = 3) : IRequest<List<OrganizationsByCategoryDto>>;

	public class GetOrganizationsByCategoryQueryHandler : IRequestHandler<GetOrganizationsByCategoryQuery, List<OrganizationsByCategoryDto>>
	{
		private readonly IOrganizationRepository _organizationRepository;
		private readonly IWorkshopRepository _workshopRepository;
		private readonly ICategoryRepository _categoryRepository; 

		public GetOrganizationsByCategoryQueryHandler(
			IOrganizationRepository organizationRepository,
			IWorkshopRepository workshopRepository,
			ICategoryRepository categoryRepository)
		{
			_organizationRepository = organizationRepository;
			_workshopRepository = workshopRepository;
			_categoryRepository = categoryRepository; 
		}

		public async Task<List<OrganizationsByCategoryDto>> Handle(GetOrganizationsByCategoryQuery request, CancellationToken cancellationToken)
		{
			var result = new List<OrganizationsByCategoryDto>();

			// Process each category
			foreach (var categoryId in request.CategoryIds)
			{
				// Get category name
				var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
				if (category == null)
					continue;

				// Get organization IDs that have active workshops in this category
				var organizationIds = await _workshopRepository.GetOrganizationIdsByCategoryAsync(categoryId, request.LimitPerCategory, cancellationToken);

				if (!organizationIds.Any())
					continue;

				// Get organizations by IDs
				var organizations = await _organizationRepository.GetOrganizationsByIdsAsync(organizationIds, cancellationToken);

				var organizationsDto = organizations.Select(org => new OrganizationDto
				{
					Id = org.Id,
					Name = org.Name,
					Description = org.Description,
					LogoUrl = org.LogoUrl,
					ContactEmail = org.ContactEmail,
					ContactPhone = org.ContactPhone,
					Street = org.Street,
					Commune = org.Commune,
					Province = org.Province
				}).ToList();

				result.Add(new OrganizationsByCategoryDto
				{
					CategoryId = categoryId,
					CategoryName = category.Name,
					Organizations = organizationsDto
				});
			}

			return result;
		}
	}
}

