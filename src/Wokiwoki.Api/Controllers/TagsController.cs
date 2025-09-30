using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wokiwoki.Application.Features.Tags.Queries;
using Wokiwoki.Application.Features.Tags.Queries.GetTagsByCategory;

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

		[HttpGet]
		public async Task<ActionResult<List<TagDto>>> GetTagsByCategory([FromQuery] Guid categoryId)
		{
			return await _mediator.Send(new GetTagsByCategoryQuery(categoryId));
		}
	}
}
