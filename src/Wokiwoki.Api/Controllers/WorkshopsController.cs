using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wokiwoki.Application.Features.Workshops.Commands.CreateWorkshop;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/workshops")]
	[ApiController]
	public class WorkshopsController : ControllerBase
	{
		private readonly IMediator _mediator;

		public WorkshopsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromForm] CreateWorkshopCommand command)
		{
			var id = await _mediator.Send(command);
			//return CreatedAtAction(nameof(Get), new { id }, null); // nao co get thi de 
			return Created($"/api/workshops/{id}", new { id });
		}
	}
}
