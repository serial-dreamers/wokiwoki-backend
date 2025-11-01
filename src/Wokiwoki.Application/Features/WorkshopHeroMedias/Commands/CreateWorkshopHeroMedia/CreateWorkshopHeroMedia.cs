using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Utils;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.WorkshopHeroMedias.Commands.CreateWorkshopHeroMedia
{
	public record CreateWorkshopHeroMediaCommand
	(
		HeroMediaType HeroType,
		Guid WorkshopId,
		Guid MediaId
	) : IRequest<Guid>;

	public class CreateWorkshopHeroMediaCommandHandler : IRequestHandler<CreateWorkshopHeroMediaCommand, Guid>
	{
		private readonly IWorkshopHeroMediaRepository _workshopHeroMediaRepository;
		private readonly IUuidService _uuidService;
		private readonly IUserContext _userContext;

		public CreateWorkshopHeroMediaCommandHandler(IWorkshopHeroMediaRepository workshopHeroMediaRepository, IUuidService uuidService, IUserContext userContext)
		{
			_workshopHeroMediaRepository = workshopHeroMediaRepository;
			_uuidService = uuidService;
			_userContext = userContext;
		}
		public async Task<Guid> Handle(CreateWorkshopHeroMediaCommand request, CancellationToken cancellationToken)
		{
			var id = _uuidService.NewGuid();
			var workshopHeroMedia = new WorkshopHeroMedia
			{
				Id = id, 
				WorkshopId = request.WorkshopId,
				HeroType = request.HeroType,
				MediaId = request.MediaId,
				IsActive = true,
				Created = TimeHelper.NowInVietnam(),
				CreatedBy = _userContext.UserId
			};
			var workshopHeroMediaCreated = await _workshopHeroMediaRepository.CreateAsync(
			 workshopHeroMedia
			);

			return workshopHeroMediaCreated.Id;
		}
	}
}
