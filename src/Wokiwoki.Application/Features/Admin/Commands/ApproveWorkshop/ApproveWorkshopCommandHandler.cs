using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.Admin.Commands.ApproveWorkshop
{
	public class ApproveWorkshopCommandHandler : IRequestHandler<ApproveWorkshopCommand, Result>
	{
		private readonly IWorkshopRepository _workshopRepository;

		public ApproveWorkshopCommandHandler(IWorkshopRepository workshopRepository)
		{
			_workshopRepository = workshopRepository;
		}

		public async Task<Result> Handle(ApproveWorkshopCommand request, CancellationToken cancellationToken)
		{
			var workshop = await _workshopRepository.GetByIdAsync(request.WorkshopId, cancellationToken);
			
			if (workshop == null)
				return Result.Failure(new[] { "Workshop not found" });

			workshop.Status = WorkshopStatus.Published;
			workshop.Reason = null; // Clear any previous rejection reason
			workshop.LastModified = DateTime.UtcNow;

			await _workshopRepository.UpdateAsync(workshop.Id, workshop, cancellationToken);

			return Result.Success();
		}
	}
}

