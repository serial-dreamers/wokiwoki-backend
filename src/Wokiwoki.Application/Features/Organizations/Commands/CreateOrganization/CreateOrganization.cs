using MediatR; 
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.Organizations.Commands.CreateOrganization
{
	public record CreateOrganizationCommand(
		string Name,
		string Description,
		string ContactEmail, 
		string ContactPhone,
		string Street,
		string Commune,
		string Province,
		string LogoUrl
		) : IRequest<Guid>;

	public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, Guid>
	{
		private readonly IOrganizationRepository _organizationRepository;
		private readonly IUuidService _uuidService;
		public CreateOrganizationCommandHandler(IOrganizationRepository organizationRepository,
			IUuidService uuidService)
		{
			_organizationRepository = organizationRepository;
			_uuidService = uuidService;
		}
		public async Task<Guid> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
		{
			var Id = _uuidService.NewGuid();
			var organization = new Organization
			{
				Id = Id,
				Name =  request.Name,
				Description = request.Description,
				ContactEmail = request.ContactEmail,
				ContactPhone = request.ContactPhone,
				Street = request.Street,
				Commune = request.Commune,
				Province = request.Province,
				LogoUrl = request.LogoUrl,
				Created = DateTime.UtcNow,
				IsActive = true,
				FollowerCount= 0,
				CreatedBy = "system",
				OwnerId = "00000000-0000-0000-0000-000000000001",
				Status = OrganizationStatus.Pending
			}; 

			var createdOrganization = await _organizationRepository.CreateAsync(organization, cancellationToken);
			if (createdOrganization != null)
			{
				return createdOrganization.Id;
			}
			else
			{
				throw new Exception("Failed to create organization");
			}
		}
	}
}
