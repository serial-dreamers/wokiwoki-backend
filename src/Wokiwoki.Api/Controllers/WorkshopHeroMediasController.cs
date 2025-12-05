using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Application.Features.WorkshopHeroMedias;
using Wokiwoki.Application.Features.WorkshopHeroMedias.Commands.CreateWorkshopHeroMedia;
using Wokiwoki.Application.Features.WorkshopHeroMedias.Queries.GetHeroMediaById;
using Wokiwoki.Application.Features.WorkshopHeroMedias.Queries.GetHeroMedias;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WorkshopHeroMediasController : ControllerBase
	{
		private readonly IMediator _mediator;
		public WorkshopHeroMediasController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("{workshopId}/heroes")]
		[SwaggerOperation(
			Summary = "Get hero media list for a specific workshop",
			Description = "Retrieve all hero media items (images/videos) associated with the given workshop ID."
		)]
		[ProducesResponseType(typeof(List<WorkshopHeroMedia>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetHeroMediasByWorkshopId(Guid workshopId)
		{
			var result = await _mediator.Send(new GetHeroMediasByWorkshopIdQuery(workshopId));

			//if (result == null || !result.Any())
			//	return NotFound(new { message = "No hero media found for this workshop." });

			return Ok(result);
		}

		[HttpPost("{workshopId}/hero")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Create hero media for a workshop",
			Description = "Create a new hero media record linked to a specific workshop and media item."
		)]
		[ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateHeroMedia([FromBody] CreateWorkshopHeroMediaCommand command)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var id = await _mediator.Send(command);
			return CreatedAtAction(nameof(GetHeroMediaById), new { id }, new { id });
		}

		 
		[HttpGet("hero/{id:guid}")]
		[SwaggerOperation(
			Summary = "Get hero media by ID",
			Description = "Retrieve details of a specific hero media record."
		)]
		[ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetHeroMediaById(Guid id)
		{
			var heroMedia = await _mediator.Send(new GetHeroMediaByIdQuery(id));
			if (heroMedia == null)
				return NotFound(new { message = "Hero media not found." });

			return Ok(heroMedia);
		}


		 
		[Authorize]		
		[HttpPut("{workshopId}/heroes/sync")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Sync hero media for a workshop",
			Description = "Add new hero media, update existing ones, and deactivate removed items in bulk."
		)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> SyncHeroMedia(Guid workshopId, [FromBody] SyncWorkshopHeroMediaCommand command)
		{
			if (workshopId != command.WorkshopId)
				return BadRequest(new { message = "WorkshopId in URL does not match body" });

			await _mediator.Send(command);
			return NoContent(); // 204, vì command trả Unit
		}
		 

	}
}
