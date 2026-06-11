using MediatR;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.Admin.Commands.RejectWorkshop
{
	public record RejectWorkshopCommand(Guid WorkshopId, string Reason) : IRequest<Result>;
}

