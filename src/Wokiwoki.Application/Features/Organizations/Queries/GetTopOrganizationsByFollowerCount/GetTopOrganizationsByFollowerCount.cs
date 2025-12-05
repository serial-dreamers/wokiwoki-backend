using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Organizations.Queries.GetTopOrganizationsByFollowerCount
{
	public record GetTopOrganizationsByFollowerCountQuery(int Limit = 6) : IRequest<List<OrganizationDto>>;

	public class GetTopOrganizationsByFollowerCountQueryHandler : IRequestHandler<GetTopOrganizationsByFollowerCountQuery, List<OrganizationDto>>
	{
		private readonly IOrganizationRepository _organizationRepository;

		public GetTopOrganizationsByFollowerCountQueryHandler(IOrganizationRepository organizationRepository)
		{
			_organizationRepository = organizationRepository;
		}

		public async Task<List<OrganizationDto>> Handle(GetTopOrganizationsByFollowerCountQuery request, CancellationToken cancellationToken)
		{
			var organizations = await _organizationRepository.GetTopOrganizationsByFollowerCountAsync(request.Limit, cancellationToken);

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
				Province = org.Province,
				FollowerCount = org.FollowerCount
			}).ToList();

			return organizationsDto;
		}
	}
}

