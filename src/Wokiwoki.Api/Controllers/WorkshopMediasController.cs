using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Api.Request;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.WorkshopMedias.Commands.CreateWorkshopMedia;
using Wokiwoki.Application.Features.WorkshopMedias.Queries.GetWorkshopMediaById;
using Wokiwoki.Application.Features.WorkshopMedias.Queries.GetWorkshopMedias;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WorkshopMediasController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly IBlobStorageService _blobStorageService;


		public WorkshopMediasController(IMediator mediator, IBlobStorageService blobStorageService)
		{
			_mediator = mediator;
			_blobStorageService = blobStorageService;

		}

		/// <summary>
		/// Upload a new media (image/video) for a workshop.
		/// </summary>
		/// <param name="request">The media upload request containing the file and associated workshop ID.</param>
		/// <returns>Returns the ID of the created workshop media.</returns>
		[Authorize]		
		[HttpPost]
		[Consumes("multipart/form-data")]
		[SwaggerOperation(
			Summary = "Upload workshop media",
			Description = "Upload a new media file (image or video) and associate it with a specific workshop."
		)]
		[ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Create([FromForm] CreateWorkshopMediaRequest request)
		{
			if (request.LogoFile == null)
				return BadRequest("Không có file nào được tải lên.");

			string? imgUrl = string.Empty;
			MediaType mediaType = MediaType.Image;
			var ext = Path.GetExtension(request.LogoFile.FileName).ToLowerInvariant(); 
			if (new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }.Contains(ext))
			{
				mediaType = MediaType.Image;
			}
			else if (new[] { ".mp4", ".mov", ".avi", ".mkv" }.Contains(ext))
			{
				mediaType = MediaType.Video;
			}
			else
			{
				return BadRequest("Định dạng file không hợp lệ. Chỉ hỗ trợ ảnh (.jpg, .png, .gif, .webp) và video (.mp4, .mov, .avi, .mkv).");
			} 

			var command = new CreateWorkshopMediaCommand( 
				request.WorkshopId,
				mediaType,
				request.LogoFile.FileName,
				request.LogoFile.ContentType,
				request.LogoFile.Length,
				request.LogoFile.OpenReadStream()
			);

			var id = await _mediator.Send(command);
			return CreatedAtAction(nameof(GetById), new { id }, new { id });
		}

		/// <summary>
		/// Get a specific workshop media by its ID.
		/// </summary>
		/// <param name="id">The ID of the workshop media.</param>
		/// <returns>Returns the workshop media details.</returns>
		[HttpGet("{id:guid}")]
		[SwaggerOperation(
			Summary = "Get workshop media by ID",
			Description = "Retrieve the details of a specific workshop media using its unique ID."
		)]
		[ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetById(Guid id)
		{
			var workshopMedia = await _mediator.Send(new GetWorkshopMediaByIdQuery(id));
			if (workshopMedia == null)
				return NotFound();
			return Ok(workshopMedia);
		}

		/// <summary>
		/// Get the list of active media (images, videos, etc.) for a specific workshop.
		/// </summary>
		/// <param name="workshopId">The ID of the workshop</param>
		/// <returns>List of media as WorkshopMediaDto</returns>
		[HttpGet("{workshopId}/medias")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Get active media for a workshop",
			Description = "Retrieve all active media (images, videos) associated with a specific workshop."
		)]
		[ProducesResponseType(typeof(List<WorkshopMediaDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetMediasByWorkshopId(Guid workshopId)
		{
			var medias = await _mediator.Send(new GetMediasByWorkshopIdQuery(workshopId));

			if (medias == null || !medias.Any())
				return NotFound(new { message = "No active media found for this workshop." });

			return Ok(medias);
		}


	}
}
