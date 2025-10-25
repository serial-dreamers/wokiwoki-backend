using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Application.Features.WorkshopHeroMedias;
using Wokiwoki.Application.Features.WorkshopHeroMedias.Commands.CreateWorkshopHeroMedia;
using Wokiwoki.Application.Features.WorkshopHeroMedias.Queries.GetHeroMediaById;

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

		/// <summary>
		/// Add a new hero media (image/video) for a workshop.
		/// </summary>
		/// <param name="command">The request containing HeroType, WorkshopId, and MediaId.</param>
		/// <returns>Returns the ID of the created WorkshopHeroMedia.</returns>
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

		/// <summary>
		/// Get a hero media by its ID.
		/// </summary>
		/// <param name="id">The ID of the WorkshopHeroMedia.</param>
		/// <returns>Returns the hero media details.</returns>
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


		/// <summary>
		/// Sync a list of hero media for a workshop.
		/// </summary>
		/// <param name="command">Contains WorkshopId and a list of HeroMedias to sync.</param>
		/// <returns>No content. Syncs hero media by adding, updating, or deactivating as needed.</returns>
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
