using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Application.DTOs;
using Wokiwoki.Application.Features.Tags.Queries.GetFilterPagedTagsQuery;

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
		/// Get tags for a specific category.
		/// </summary>
		/// <remarks>
		/// Provide the categoryId as a query parameter. Returns a list of TagDto associated with the category.
		/// Example: GET /api/category?categoryId=00000000-0000-0000-0000-000000000000
		/// </remarks>
		[HttpGet]
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
