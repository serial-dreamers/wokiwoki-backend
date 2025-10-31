using AutoMapper;
using MediatR; 
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Organizations.Queries.GetOrganizationById
{
	public sealed record GetOrganizationByIdQuery(Guid Id) : IRequest<OrganizationDto?>;

	public class GetOrganizationByIdQueryHandler : IRequestHandler<GetOrganizationByIdQuery, OrganizationDto?>
	{
		private readonly IOrganizationRepository _organizationRepository;
		private readonly IMapper _mapper;
		public GetOrganizationByIdQueryHandler(IOrganizationRepository organizationRepository, IMapper mapper)
		{
			_organizationRepository = organizationRepository;
			_mapper = mapper;	
		}
		public async Task<OrganizationDto?> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
		{
			var organization = await  _organizationRepository.GetByIdAsync(request.Id, cancellationToken);
			return _mapper.Map<OrganizationDto?>(organization);
		}
	}
}
