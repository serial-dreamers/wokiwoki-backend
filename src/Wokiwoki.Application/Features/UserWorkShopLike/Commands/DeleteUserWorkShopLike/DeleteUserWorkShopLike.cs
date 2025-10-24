using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;

namespace Wokiwoki.Application.Features.UserWorkShopLike.Commands.DeleteUserWorkShopLike
{
	public record DeleteUserWorkshopLikeCommand(Guid WorkshopId) : IRequest;

	public class DeleteUserWorkshopLikeCommandHandler : IRequestHandler<DeleteUserWorkshopLikeCommand>
	{
		private readonly IUserWorkshopLikeRepository _likeRepo;
		private readonly IWorkshopRepository _workshopRepo;
		private readonly IUserContext _userContext;

		public DeleteUserWorkshopLikeCommandHandler(
		IUserWorkshopLikeRepository likeRepo,
		IWorkshopRepository workshopRepo,
		IUserContext userContext)
		{
			_likeRepo = likeRepo;
			_workshopRepo = workshopRepo;
			_userContext = userContext;
		}

		public async Task Handle(DeleteUserWorkshopLikeCommand request, CancellationToken cancellationToken)
		{
			var userId = _userContext.UserId ?? throw new UnauthorizedAccessException();

			var workshop = await _workshopRepo.GetByIdAsync(request.WorkshopId, cancellationToken);
			if (workshop == null)
				throw new KeyNotFoundException("Workshop not found");


			var existingLike = await _likeRepo.GetExistsAsync(userId, request.WorkshopId, cancellationToken);
			if (existingLike == null)
				return;

			await _likeRepo.DeleteAsync(existingLike.Id, cancellationToken);

			await _workshopRepo.DecrementLikeCountAsync(request.WorkshopId, cancellationToken);
		}
		 
	}
}
