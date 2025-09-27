using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wokiwoki.Application.Features.Categories.Queries.GetCategories;

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

		[HttpGet]
		public async Task<ActionResult<List<CategoryDto>>> Get(CancellationToken cancellationToken)
		{
			//var query = new GetCategoriesQuery();
			//var categories = await _mediator.Send(query, cancellationToken);
			var result = await _mediator.Send(new GetCategoriesQuery(), cancellationToken);
			return Ok(result);
		}
	}
}
