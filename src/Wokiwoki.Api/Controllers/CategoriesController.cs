using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

		/// <summary>
		/// Get category details by ID
		/// </summary>
		/// <param name="id">Category ID</param>
		[HttpGet("{id:guid}")]
		[SwaggerOperation(Summary = "Get category by ID", Description = "Fetch detailed information for a single category.")]
		[ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetById(Guid id)
		{
			var category = await _mediator.Send(new GetCategoryByIdQuery(id));

			if (category == null)
				return NotFound();

			return Ok(category);
		}


		/// <summary>
		/// Get all categories
		/// </summary>
		[HttpGet]
		[SwaggerOperation(Summary = "Get all categories", Description = "Fetch list of all available categories.")]
		[ProducesResponseType(typeof(List<Category>), StatusCodes.Status200OK)]
		public async Task<ActionResult<List<CategoryDto>>> Get(CancellationToken cancellationToken)
		{
			//var query = new GetCategoriesQuery();
			//var categories = await _mediator.Send(query, cancellationToken);
			var result = await _mediator.Send(new GetCategoriesQuery(), cancellationToken);
			return Ok(result);
		}

		/// <summary>
		/// Create a new category
		/// </summary>
		/// <param name="command">Category creation request</param>
		/// <returns>Created category Id</returns>
		[HttpPost]
		[Consumes("application/json")]
		[SwaggerOperation(Summary = "Create a new category", Description = "Add a new category to the system.")]
		[ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
		{
			var categoryId = await _mediator.Send(command);
			return Ok(new { id = categoryId, message = "Category created successfully." });
		}

		/// <summary>
		/// Update an existing category
		/// </summary>
		/// <param name="id">Category ID</param>
		/// <param name="command">Updated category data</param>
		[HttpPut("{id:guid}")]
		[Consumes("application/json")]
		[SwaggerOperation(Summary = "Update an existing category", Description = "Modify category name, description, or images.")]
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

		/// <summary>
		/// Delete category by ID
		/// </summary>
		/// <param name="id">Category ID</param>
		[HttpDelete("{id:guid}")]
		[SwaggerOperation(Summary = "Delete a category", Description = "Remove a category from the system.")]
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
