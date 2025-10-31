using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Tags.Commands.CreateTag;
using Wokiwoki.Application.Features.Tags.Commands.DeleteTag;
using Wokiwoki.Application.Features.Tags.Commands.UpdateTag;
using Wokiwoki.Application.Features.Tags.Queries.GetFilterPagedTagsQuery;
using Wokiwoki.Application.Features.Tags.Queries.GetTagById;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TagsController : ControllerBase
	{
		private readonly ISender _mediator;
		public TagsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Get a tag by ID.
		/// </summary>
		/// <param name="id">The tag ID.</param>
		[HttpGet("{id:guid}")]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Get tag by ID",
			Description = "Fetches a single tag by its unique identifier.",
			Tags = new[] { "Tags" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Tag found", typeof(TagDto))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Tag not found")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var tag = await _mediator.Send(new GetTagByIdQuery(id));
			if (tag == null)
				return NotFound(new { Message = $"Tag with ID '{id}' not found." });

			return Ok(tag);
		}


		/// <summary>
		/// Create a new tag.
		/// </summary>
		/// <remarks>
		/// Creates a tag with the given name, description, and icon URL.
		/// Returns the ID of the newly created tag.
		/// Requires the user to have the 'Admin' role.
		/// </remarks>
		/// <param name="command">The tag creation command.</param>
		[Authorize(Policy = "RequireAdminRole")]
		[HttpPost]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Create a new tag",
			Description = "Creates a new tag and returns its ID. Only users with the 'Admin' role are authorized to perform this action.",
			Tags = new[] { "Tags" })]
		[SwaggerResponse(StatusCodes.Status201Created, "Tag created successfully", typeof(Guid))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data")]
		[SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have the required 'Admin' role")]
		public async Task<IActionResult> CreateTag([FromBody] CreateTagCommand command)
		{
			var tagId = await _mediator.Send(command);
			return CreatedAtAction(nameof(GetById), new { id = tagId }, new { id = tagId });
		}


		/// <summary>
		/// Update an existing tag.
		/// </summary>
		/// <remarks>
		/// Updates the name, description, and icon URL of a tag.
		/// Requires the user to have the 'Admin' role.
		/// </remarks>
		/// <param name="id">The ID of the tag to update.</param>
		/// <param name="command">The update request body.</param>
		[Authorize(Policy = "RequireAdminRole")]
		[HttpPut("{id:guid}")]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Update a tag",
			Description = "Updates the specified tag by ID. Only users with the 'Admin' role are authorized to perform this action.",
			Tags = new[] { "Tags" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Tag updated successfully", typeof(TagDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Mismatched tag ID in URL and body")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Tag not found")]
		[SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have the required 'Admin' role")]
		public async Task<IActionResult> UpdateTag(Guid id, [FromBody] UpdateTagCommand command)
		{
			if (id != command.Id)
				return BadRequest("Mismatched tag ID in URL and body.");

			var updatedTag = await _mediator.Send(command);
			return Ok(updatedTag);
		}


		/// <summary>
		/// Delete a tag by ID.
		/// </summary>
		/// <remarks>
		/// Deletes the tag specified by its unique identifier.
		/// Requires the user to have the 'Admin' role.
		/// </remarks>
		/// <param name="id">The ID of the tag to delete.</param>
		[Authorize(Policy = "RequireAdminRole")]
		[HttpDelete("{id:guid}")]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Delete a tag",
			Description = "Deletes the tag with the given ID. Only users with the 'Admin' role are authorized to perform this action.",
			Tags = new[] { "Tags" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Tag deleted successfully")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Tag not found")]
		[SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have the required 'Admin' role")]
		public async Task<IActionResult> DeleteTag(Guid id)
		{
			var result = await _mediator.Send(new DeleteTagCommand(id));

			if (!result)
				return NotFound(new { Message = $"Tag with id '{id}' not found." });

			return Ok(new { Message = "Tag deleted successfully." });
		}


		/// <summary>
		/// Get tags for a specific category.
		/// </summary>
		/// <remarks>
		/// Provide the categoryId as a query parameter. Returns a list of TagDto associated with the category.
		/// Example: GET /api/category?categoryId=00000000-0000-0000-0000-000000000000
		/// </remarks> 
		[HttpGet("by-category")]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Get tags by category",
			Description = "Retrieve all tags that belong to the specified category.",
			Tags = new[] { "Tags" })]
		[ProducesResponseType(typeof(List<TagDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<List<TagDto>>> GetTagsByCategory([FromQuery] Guid categoryId)
		{
			if (categoryId == Guid.Empty)
				return BadRequest("Invalid categoryId.");

			var tags = await _mediator.Send(new GetTagsByCategoryQuery(categoryId));
			return Ok(tags);
		}

	}
}
