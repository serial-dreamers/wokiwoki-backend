using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Organizations.Queries.GetMyOrganization
{
	public class GetMyOrganizationQueryHandler : IRequestHandler<GetMyOrganizationQuery, OrganizationDto?>
	{
		private readonly IOrganizationRepository _organizationRepository;
		private readonly IUserContext _userContext;

		public GetMyOrganizationQueryHandler(
			IOrganizationRepository organizationRepository,
			IUserContext userContext)
		{
			_organizationRepository = organizationRepository;
			_userContext = userContext;
		}

		public async Task<OrganizationDto?> Handle(GetMyOrganizationQuery request, CancellationToken cancellationToken)
		{
			var userId = _userContext.UserId;
			if (string.IsNullOrEmpty(userId))
				throw new UnauthorizedAccessException("User not authenticated");

			var organizationId = await _organizationRepository.GetOrganizationIdByUserIdAsync(userId);
			if (!organizationId.HasValue)
				return null;

			var organization = await _organizationRepository.GetByIdAsync(organizationId.Value, cancellationToken);
			if (organization == null)
				return null;

			return new OrganizationDto
			{
				Id = organization.Id,
				Name = organization.Name,
				Description = organization.Description,
				LogoUrl = organization.LogoUrl,
				ContactEmail = organization.ContactEmail,
				ContactPhone = organization.ContactPhone,
				Street = organization.Street,
				Commune = organization.Commune,
				Province = organization.Province,
				FollowerCount = organization.FollowerCount
			};
		}
	}
}

