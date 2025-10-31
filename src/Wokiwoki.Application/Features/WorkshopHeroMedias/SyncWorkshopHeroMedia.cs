using MediatR;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.WorkshopHeroMedias
{
	public class HeroMediaDto
	{
		public Guid? Id { get; set; }
		public HeroMediaType HeroType { get; set; }
		public Guid? MediaId { get; set; }
		public bool IsActive { get; set; } = true;
	}
	public record SyncWorkshopHeroMediaCommand(
		Guid WorkshopId,
		List<HeroMediaDto> HeroMedias
	) : IRequest<Unit>;
	public class SyncWorkshopHeroMediaCommandHandler : IRequestHandler<SyncWorkshopHeroMediaCommand, Unit>
	{
		private readonly IWorkshopHeroMediaRepository _heroMediaRepository;
		private readonly IWorkshopRepository _workshopRepository;

		public SyncWorkshopHeroMediaCommandHandler(IWorkshopHeroMediaRepository workshopHeroMediaRepository,
			IWorkshopRepository workshopRepository )
		{
			_heroMediaRepository = workshopHeroMediaRepository;
			_workshopRepository = workshopRepository;
		}
		public async Task<Unit> Handle(SyncWorkshopHeroMediaCommand request, CancellationToken cancellationToken)
		{
			var workshop = await _workshopRepository.GetByIdAsync(request.WorkshopId);
			if (workshop == null)
				throw new KeyNotFoundException("Workshop not found");

			var heroMediaDtos = request.HeroMedias.Select(x => new WorkshopHeroMediaDto
			{
				Id = x.Id,
				HeroType = x.HeroType,
				MediaId = x.MediaId, 
			}).ToList();

			await _heroMediaRepository.SyncHeroMediaAsync(
			request.WorkshopId,
			heroMediaDtos,
			cancellationToken);

			return Unit.Value;
		}
	}
}
