using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wokiwoki.Application.Features.Workshops.Commands.CreateWorkshop;
using Wokiwoki.Application.Features.Workshops.Queries.GetWorkshop;
using Wokiwoki.Application.Features.Workshops.Queries.SearchWorkshop;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
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
		[HttpPost]
		[Route("/Search")]
		public async Task<IActionResult> Search([FromForm] SearchWorkshopQuery request)
		{
			var result = await _mediator.Send(request);
			return Ok(result);
		}
		[HttpGet]
		public async Task<IActionResult> GetAll([FromQuery] GetAllWorkshopQuery request)
		{
			
			var result = await _mediator.Send(request);
			return Ok(result);
		}
	}
}
