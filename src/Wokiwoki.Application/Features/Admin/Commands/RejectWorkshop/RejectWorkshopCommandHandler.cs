using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.Admin.Commands.RejectWorkshop
{
	public class RejectWorkshopCommandHandler : IRequestHandler<RejectWorkshopCommand, Result>
	{
		private readonly IWorkshopRepository _workshopRepository;

		public RejectWorkshopCommandHandler(IWorkshopRepository workshopRepository)
		{
			_workshopRepository = workshopRepository;
		}

		public async Task<Result> Handle(RejectWorkshopCommand request, CancellationToken cancellationToken)
		{
			var workshop = await _workshopRepository.GetByIdAsync(request.WorkshopId, cancellationToken);
			
			if (workshop == null)
				return Result.Failure(new[] { "Workshop not found" });

			if (string.IsNullOrWhiteSpace(request.Reason))
				return Result.Failure(new[] { "Reason is required for rejection" });

			workshop.Status = WorkshopStatus.Hidden;
			workshop.Reason = request.Reason;
			workshop.LastModified = DateTime.UtcNow;

			await _workshopRepository.UpdateAsync(workshop.Id, workshop, cancellationToken);

			return Result.Success();
		}
	}
}

