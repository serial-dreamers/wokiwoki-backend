using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Utils;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.WorkshopMedias.Commands.CreateWorkshopMedia
{
	public record CreateWorkshopMediaCommand
	(   
		Guid WorkshopId,
		MediaType mediaType,
		string FileName,
		string ContentType,
		long FileLength,
		Stream FileStream
	) : IRequest<Guid>;

	public class CreateWorkshopMediaCommandHandler : IRequestHandler<CreateWorkshopMediaCommand, Guid>
	{
		private readonly IWorkshopMediaRepository _workshopMediaRepository;
		private readonly IWorkshopRepository _workshopRepository;
		private readonly IBlobStorageService _blobStorageService;
		private readonly IUuidService _uuidService;
		private readonly IUserContext _userContext;


		public CreateWorkshopMediaCommandHandler(IWorkshopMediaRepository workshopMediaRepository,
			IWorkshopRepository workshopRepository,
			IBlobStorageService blobStorageService,
			IUuidService uuidService,
			IUserContext userContext)
		{
			_workshopMediaRepository = workshopMediaRepository;
			_workshopRepository = workshopRepository;
			_blobStorageService = blobStorageService;
			_uuidService = uuidService;
			_userContext = userContext;
		}

		public async Task<Guid> Handle(CreateWorkshopMediaCommand request, CancellationToken cancellationToken)
		{
			var workshopExist = await _workshopRepository.CheckWorkshopExistById(request.WorkshopId, cancellationToken);
			if(!workshopExist)
				return Guid.Empty;


			var id = _uuidService.NewGuid();
			string imgUrl = await _blobStorageService.UploadFileAsync(
			request.FileStream,
			request.FileName,
			request.ContentType,
			request.FileLength,
			BlobContainerType.WorkshopMedia
		);
			var workshopMedia = new WorkshopMedia
			{
				Id = id,
				ImageUrl = imgUrl,
				WorkshopId = request.WorkshopId,
				MediaType = request.mediaType,
				Created = TimeHelper.NowInVietnam(),
				CreatedBy = _userContext.UserId,
				IsActive = true,
				LastModified = TimeHelper.NowInVietnam(),
				LastModifiedBy = _userContext.UserId,
			};
			var workshopMediaCreated = await _workshopMediaRepository.CreateAsync(workshopMedia, cancellationToken);
			if (workshopMediaCreated == null) 
				throw new Exception("Create workshop media failed");
			
			return workshopMediaCreated.Id;
		}
	}
}
