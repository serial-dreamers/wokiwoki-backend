using MediatR;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.Admin.Commands.RejectOrganization
{
	public record RejectOrganizationCommand(Guid OrganizationId, string Reason) : IRequest<Result>;
}

