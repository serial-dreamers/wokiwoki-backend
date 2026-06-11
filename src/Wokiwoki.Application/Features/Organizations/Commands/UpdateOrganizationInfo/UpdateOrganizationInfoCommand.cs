using MediatR;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.Organizations.Commands.UpdateOrganizationInfo
{
	public record UpdateOrganizationInfoCommand(
		string Name,
		string? Description,
		string? ContactEmail,
		string? ContactPhone,
		string? Street,
		string? Commune,
		string? Province
	) : IRequest<Result>;
}

