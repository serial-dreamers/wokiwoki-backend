using MediatR;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.Organizations.Commands.UpdateOrganizationLogo
{
	public record UpdateOrganizationLogoCommand(string LogoUrl) : IRequest<Result>;
}

