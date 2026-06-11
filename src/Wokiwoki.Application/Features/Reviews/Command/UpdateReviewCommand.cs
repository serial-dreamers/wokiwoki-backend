using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.Reviews.Command
{
	public sealed record UpdateReviewCommand(
		Guid ReviewId,
		string Comment,
		int Rating,
		string FileName,
		string ContentType,
		long FileLength,
		Stream FileStream
		) : IRequest<Review>;

	public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, Review>
	{
		private readonly IReviewRepository _repo;
		private readonly IBlobStorageService _blobStorageService;

		public UpdateReviewCommandHandler(IReviewRepository repo, IBlobStorageService blobStorageService)
		{
			_repo = repo;
			_blobStorageService = blobStorageService;
		}

		public async Task<Review> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
		{
			var existingReview = await _repo.GetByIdAsync(request.ReviewId);
			if (existingReview == null)
			{
				return null;
			}

			string? imgUrl = existingReview.ImageUrl;

			Console.WriteLine("File : " + request.FileLength);
			if (request.FileStream != Stream.Null && request.FileLength > 0)
			{
				var fileExtension = Path.GetExtension(request.FileName).ToLowerInvariant();
				var fileName = $"review_{existingReview.UserId}_{Guid.NewGuid()}{fileExtension}";

				imgUrl = await _blobStorageService.UploadFileAsync(
					request.FileStream,
					fileName,
					request.ContentType,
					request.FileLength,
					BlobContainerType.WorkshopMedia
				);


			}
			Console.WriteLine("File img: " + imgUrl);

			existingReview.Comment = request.Comment;
			existingReview.Rating = request.Rating;
			existingReview.ImageUrl = imgUrl;
			existingReview.LastModified = DateTime.UtcNow;

			var result = await _repo.UpdateTAsync(existingReview.Id, existingReview);
			return result;

		}
	}
}
