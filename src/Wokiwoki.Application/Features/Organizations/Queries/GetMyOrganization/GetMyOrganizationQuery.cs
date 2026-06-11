using MediatR;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Organizations.Queries.GetMyOrganization
{
	public record GetMyOrganizationQuery : IRequest<OrganizationDto?>;
}

