using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Api.Request;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Reviews.Command;
using Wokiwoki.Application.Features.Reviews.Queries;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReviewController : ControllerBase
	{
		private readonly IMediator _mediator;

		public ReviewController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		[Consumes("multipart/form-data")]
		[SwaggerOperation(
	Summary = "Create a new review for a workshop",
	Description = "Creates a review for a specific workshop and booking. Optionally uploads an image (jpg, jpeg, png, gif, webp) for the review.",
	Tags = new[] { "Reviews" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Review created successfully", typeof(ReviewDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Workshop or Booking not found")]
		public async Task<IActionResult> Create([FromForm] CreateReviewRequest request)
		{
			// Debug logging
			Console.WriteLine($"Create Review Request: WorkshopId={request.WorkshopId}, BookingId={request.BookingId}, UserId={request.UserId}");
			Console.WriteLine($"Comment: {request.Comment}, Rating: {request.Rating}");
			if (request.LogoFile != null)
			{
				Console.WriteLine($"LogoFile: Name={request.LogoFile.FileName}, Size={request.LogoFile.Length}, ContentType={request.LogoFile.ContentType}");
			}
			else
			{
				Console.WriteLine("LogoFile: null");
			}

			// Validate file if provided
			if (request.LogoFile != null && request.LogoFile.Length > 0)
			{
				var ext = Path.GetExtension(request.LogoFile.FileName).ToLowerInvariant();
				if (!new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }.Contains(ext))
				{
					return BadRequest(new { message = "Định dạng file không hợp lệ. Chỉ hỗ trợ ảnh (.jpg, .jpeg, .png, .gif, .webp)." });
				}

				if (request.LogoFile.Length > 5 * 1024 * 1024)
				{
					return BadRequest(new { message = "Kích thước file vượt quá giới hạn 5MB." });
				}
			}

			// Handle optional logo file
			string fileName = request.LogoFile != null ? request.LogoFile.FileName : string.Empty;
			string contentType = request.LogoFile != null ? request.LogoFile.ContentType : string.Empty;
			long fileLength = request.LogoFile != null ? request.LogoFile.Length : 0;
			Stream fileStream = request.LogoFile != null ? request.LogoFile.OpenReadStream() : Stream.Null;

			var command = new CreateReviewCommand(
				request.WorkshopId,
				request.BookingId,
				fileName,
				contentType,
				fileLength,
				fileStream,
				request.UserId,
				request.Comment,
				request.Rating
			);
			var result = await _mediator.Send(command);
			return Ok(result);
		}

		[HttpPut("{reviewId}")]
		[Consumes("multipart/form-data")]
		[SwaggerOperation(
	Summary = "Update an existing review",
	Description = "Updates the comment, rating, and optional image of an existing review.",
	Tags = new[] { "Reviews" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Review updated successfully", typeof(ReviewDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Review not found")]
		public async Task<IActionResult> Update(Guid reviewId, [FromForm] UpdateReviewRequest request)
		{
			// Debug logging
			Console.WriteLine($"Update Review Request: ReviewId={reviewId}");
			Console.WriteLine($"Comment: {request.Comment}, Rating: {request.Rating}");
			if (request.LogoFile != null)
			{
				Console.WriteLine($"LogoFile: Name={request.LogoFile.FileName}, Size={request.LogoFile.Length}, ContentType={request.LogoFile.ContentType}");
			}
			else
			{
				Console.WriteLine("LogoFile: null");
			}

			if (request.LogoFile != null && request.LogoFile.Length > 0)
			{
				var ext = Path.GetExtension(request.LogoFile.FileName).ToLowerInvariant();
				if (!new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }.Contains(ext))
				{
					return BadRequest(new { message = "Định dạng file không hợp lệ. Chỉ hỗ trợ ảnh (.jpg, .jpeg, .png, .gif, .webp)." });
				}

				if (request.LogoFile.Length > 5 * 1024 * 1024)
				{
					return BadRequest(new { message = "Kích thước file vượt quá giới hạn 5MB." });
				}
			}

			var command = new UpdateReviewCommand(
				reviewId,
				request.Comment,
				request.Rating,
				request.LogoFile?.FileName ?? string.Empty,
				request.LogoFile?.ContentType ?? string.Empty,
				request.LogoFile?.Length ?? 0,
				request.LogoFile?.OpenReadStream() ?? Stream.Null
			);

			var result = await _mediator.Send(command);
			if (result == null)
			{
				return NotFound();
			}

			return Ok(result);
		}

		[HttpGet("booking/{bookingId}")]
		public async Task<IActionResult> GetReviewByBookingId(Guid bookingId)
		{
			var query = new GetReviewByBookingIdQuery(bookingId);
			var result = await _mediator.Send(query);
			if (result == null)
			{
				return NotFound();
			}
			return Ok(result);
		}

		[HttpGet("workshop/{workshopId}")]
		[SwaggerOperation(
			Summary = "Get reviews for a workshop",
			Description = "Retrieves paginated reviews for a specific workshop.",
			Tags = new[] { "Reviews" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Reviews retrieved successfully", typeof(PaginatedList<ReviewDto>))]
		public async Task<IActionResult> GetReviewsByWorkshopId(Guid workshopId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
		{
			var query = new GetReviewsByWorkshopIdQuery(workshopId, pageNumber, pageSize);
			var result = await _mediator.Send(query);
			return Ok(result);
		}
	}
}
