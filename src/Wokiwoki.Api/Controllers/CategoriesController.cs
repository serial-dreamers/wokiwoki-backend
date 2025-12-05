using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Categories.Commands.CreateCategory;
using Wokiwoki.Application.Features.Categories.Commands.DeleteCategory;
using Wokiwoki.Application.Features.Categories.Commands.UpdateCategory;
using Wokiwoki.Application.Features.Categories.Queries.GetCategories;
using Wokiwoki.Application.Features.Categories.Queries.GetCategoryById;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoriesController : ControllerBase
	{
		private readonly IMediator _mediator;
		public CategoriesController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("with-tags")]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Get all categories with tags",
			Description = "Fetch all categories along with their associated tags.",
			 Tags = new[] { "Categories" }
		)]
		[ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllWithTags()
		{
			var result = await _mediator.Send(new GetCategoriesWithTagsQuery());
			return Ok(result);
		}


		[HttpGet]
		[SwaggerOperation(
			Summary = "Get all categories",
			Description = "Fetch list of all available categories.",
			Tags = new[] { "Categories" }
		)]
		[ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
		public async Task<ActionResult<List<CategoryDto>>> Get(CancellationToken cancellationToken)
		{
			var result = await _mediator.Send(new GetCategoriesQuery(), cancellationToken);
			return Ok(result);
		}


		[HttpGet("{id:guid}")]
		[SwaggerOperation(
			Summary = "Get category by ID",
			Description = "Fetch detailed information for a single category.",
			Tags = new[] { "Categories" }
		)]
		[ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetById(Guid id)
		{
			var category = await _mediator.Send(new GetCategoryByIdQuery(id));

			if (category == null)
				return NotFound();

			return Ok(category);
		}

		[Authorize(Policy = "RequireAdminRole")]
		[HttpPost]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Create a new category",
			Description = "Add a new category to the system.",
			Tags = new[] { "Categories" }
		)]
		[ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
		{
			var categoryId = await _mediator.Send(command);
			return Ok(new { id = categoryId, message = "Category created successfully." });
		}


		[Authorize(Policy = "RequireAdminRole")]
		[HttpPut("{id:guid}")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Update an existing category",
			Description = "Modify category name, description, or images.",
			Tags = new[] { "Categories" }
		)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
		{
			if (id != command.Id)
				return BadRequest("Mismatched category ID.");

			var result = await _mediator.Send(command);
			if (!result)
				return NotFound();

			return Ok(new { message = "Category updated successfully." });
		}


		[HttpDelete("{id:guid}")]
		[SwaggerOperation(
			Summary = "Delete a category",
			Description = "Remove a category from the system.",
			Tags = new[] { "Categories" }
		)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _mediator.Send(new DeleteCategoryCommand(id));
			if (!result)
				return NotFound();

			return Ok(new { message = "Category deleted successfully." });
		} 

	}
}
