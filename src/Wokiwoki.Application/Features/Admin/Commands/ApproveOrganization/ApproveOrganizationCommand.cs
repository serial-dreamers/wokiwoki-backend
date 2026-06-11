using MediatR;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.Admin.Commands.ApproveOrganization
{
	public record ApproveOrganizationCommand(Guid OrganizationId) : IRequest<Result>;
}

