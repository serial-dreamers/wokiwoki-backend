using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wokiwoki.Api.Request;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Features.WorkshopMedias.Commands.CreateWorkshopMedia;
using Wokiwoki.Application.Features.WorkshopMedias.Queries.GetWorkshopMediaById;
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

		[HttpPost]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> Create([FromForm] CreateWorkshopMediaRequest request)
		{
			string? imgUrl = string.Empty;

			if (request.LogoFile != null)
			{
				using var stream = request.LogoFile.OpenReadStream();
				imgUrl = await _blobStorageService.UploadFileAsync(
					stream,
					request.LogoFile.FileName,
					request.LogoFile.ContentType,
					request.LogoFile.Length,
					BlobContainerType.WorkshopMedia
				);
			}

			var command = new CreateWorkshopMediaCommand(
				imgUrl,
				request.WorkshopId
			);

			var id = await _mediator.Send(command);
			return CreatedAtAction(nameof(GetById), new { id }, new { id });
		}

		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var workshopMedia = await _mediator.Send(new GetWorkshopMediaByIdQuery(id));
			if (workshopMedia == null)
				return NotFound();
			return Ok(new { Id = id });
		}

	}
}
