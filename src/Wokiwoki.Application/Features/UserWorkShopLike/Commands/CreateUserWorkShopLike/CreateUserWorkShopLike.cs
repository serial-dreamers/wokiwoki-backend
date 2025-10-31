using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.UserWorkShopLike.Commands.CreateUserWorkShopLike
{
	public record CreateUserWorkshopLikeCommand(Guid WorkshopId) : IRequest<Guid>; 

	public class CreateUserWorkShopLikeCommandHandler : IRequestHandler<CreateUserWorkshopLikeCommand, Guid>
	{
		private readonly IUserWorkshopLikeRepository _likeRepo;
		private readonly IWorkshopRepository _workshopRepo;
		private readonly IUserContext _userContext;
		private readonly IUuidService _uuidService;

		public CreateUserWorkShopLikeCommandHandler(
	   IUserWorkshopLikeRepository likeRepo,
	   IWorkshopRepository workshopRepo,
	   IUserContext userContext,
	   IUuidService IUuidService)
		{
			_likeRepo = likeRepo;
			_workshopRepo = workshopRepo;
			_userContext = userContext;
			_uuidService = IUuidService;
		}

		public async Task<Guid> Handle(CreateUserWorkshopLikeCommand request, CancellationToken cancellationToken)
		{
			var userId = _userContext.UserId ?? throw new UnauthorizedAccessException();

			var workshop = await _workshopRepo.GetByIdAsync(request.WorkshopId, cancellationToken);
			if (workshop == null)
				throw new KeyNotFoundException("Workshop not found");

			if (await _likeRepo.ExistsAsync(userId, request.WorkshopId, cancellationToken)) 
				throw new InvalidOperationException("User has already liked this workshop");

			var like = new UserWorkshopLike
			{
				Id = _uuidService.NewGuid(),
				UserId = userId,
				WorkshopId = request.WorkshopId
			};

			var newItem = await _likeRepo.CreateAsync(like, cancellationToken); 

			await _workshopRepo
				.IncrementLikeCountAsync(request.WorkshopId, cancellationToken);

			return newItem.Id;
		} 
	}
}
