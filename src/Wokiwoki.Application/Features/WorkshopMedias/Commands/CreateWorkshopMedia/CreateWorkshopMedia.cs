using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.WorkshopMedias.Commands.CreateWorkshopMedia
{
	public record CreateWorkshopMediaCommand
	(  
		string ImageUrl,
		Guid WorkshopId
	): IRequest<Guid>;

	public class CreateWorkshopMediaCommandHandler : IRequestHandler<CreateWorkshopMediaCommand, Guid>
	{
		private readonly IWorkshopMediaRepository _workshopMediaRepository;
		private readonly IUuidService _uuidService;
		public CreateWorkshopMediaCommandHandler(IWorkshopMediaRepository workshopMediaRepository,
			IUuidService uuidService)
		{
			_workshopMediaRepository = workshopMediaRepository;
			_uuidService = uuidService;
		}
		public async Task<Guid> Handle(CreateWorkshopMediaCommand request, CancellationToken cancellationToken)
		{
			var id = _uuidService.NewGuid();
			var workshopMedia = new WorkshopMedia
			{
				Id = id,
				ImageUrl = request.ImageUrl,
				WorkshopId = request.WorkshopId,
				Created = DateTime.UtcNow,
				CreatedBy = "00000000-0000-0000-0000-000000000001",
				IsActive = true,
				LastModified = DateTime.UtcNow,
				LastModifiedBy = "00000000-0000-0000-0000-000000000001"
			};
			var workshopMediaCreated = await _workshopMediaRepository.CreateAsync(workshopMedia, cancellationToken);
			if (workshopMediaCreated == null) 
				throw new Exception("Create workshop media failed");
			
			return workshopMediaCreated.Id;
		}
	}
}
