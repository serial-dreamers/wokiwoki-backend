using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.Users.Commands.UpdateUserAvatar
{
	public record UpdateUserAvatarCommand(
		string FileName,
		string ContentType,
		long FileSize,
		Stream FileStream
	) : IRequest<Result<string>>;

	public class UpdateUserAvatarCommandHandler : IRequestHandler<UpdateUserAvatarCommand, Result<string>>
	{
		private readonly IIdentityService _identityService;
		private readonly IUserContext _userContext;
		private readonly IBlobStorageService _blobStorageService;

		public UpdateUserAvatarCommandHandler(
			IIdentityService identityService,
			IUserContext userContext,
			IBlobStorageService blobStorageService)
		{
			_identityService = identityService;
			_userContext = userContext;
			_blobStorageService = blobStorageService;
		}

		public async Task<Result<string>> Handle(UpdateUserAvatarCommand request, CancellationToken cancellationToken)
		{
			var userId = _userContext.UserId;

			try
			{
				// Upload to blob storage
				var fileExtension = Path.GetExtension(request.FileName).ToLowerInvariant();
				var fileName = $"{userId}_{Guid.NewGuid()}{fileExtension}";
				
				var imageUrl = await _blobStorageService.UploadFileAsync(
					request.FileStream,
					fileName,
					request.ContentType,
					request.FileSize,
					BlobContainerType.WorkshopMedia
				);

				// Update user avatar URL
				var updateResult = await _identityService.UpdateUserAvatarAsync(userId, imageUrl);
				if (!updateResult.Succeeded)
					return Result<string>.Failure(updateResult.Errors);

				return Result<string>.Success(imageUrl);
			}
			catch (Exception ex)
			{
				return Result<string>.Failure(new[] { $"Error uploading avatar: {ex.Message}" });
			}
		}
	}
}

