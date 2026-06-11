using MediatR;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Application.Features.Admin.Commands.ApproveWorkshop
{
	public record ApproveWorkshopCommand(Guid WorkshopId) : IRequest<Result>;
}

