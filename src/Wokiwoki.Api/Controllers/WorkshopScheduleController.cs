using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Application.Features.WorkshopSchedules.Commands.CreateSchedule;
using Wokiwoki.Application.Features.WorkshopSchedules.Queries.GetSchedule;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WorkshopScheduleController : ControllerBase
	{
		private readonly IMediator _mediator;

		public WorkshopScheduleController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Get a specific workshop schedule by ID.
		/// </summary>
		[HttpGet("{id}")]
		[SwaggerOperation(
			Summary = "Get workshop schedule by ID",
			Description = "Retrieves details of a specific workshop schedule based on its unique identifier.",
			Tags = new[] { "Schedules" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Schedule retrieved successfully", typeof(WorkshopSchedule))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid ID parameter")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Schedule not found")]
		[SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error while retrieving schedule")]
		public async Task<IActionResult> GetById(Guid id)
		{
			if (id == Guid.Empty)
				return BadRequest("Invalid ID");

			var result = await _mediator.Send(new GetScheduleByIdQuery(id));
			if (result == null)
				return NotFound("Schedule not found");

			return Ok(result);
		}

		/// <summary>
		/// Create a new workshop schedule.
		/// </summary>
		[HttpPost]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Create new workshop schedule",
			Description = "Creates a new schedule for a specific workshop with recurrence settings (daily, weekly, monthly).",
			Tags = new[] { "Schedules" })]
		[SwaggerResponse(StatusCodes.Status201Created, "Schedule created successfully", typeof(WorkshopSchedule))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request body or missing required fields")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Workshop not found")]
		[SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error while creating schedule")]
		public async Task<IActionResult> Create([FromBody] CreateScheduleCommand command)
		{
			if (command == null)
				return BadRequest("Request is null");

			try
			{
				var result = await _mediator.Send(command);
				return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

	}
}

