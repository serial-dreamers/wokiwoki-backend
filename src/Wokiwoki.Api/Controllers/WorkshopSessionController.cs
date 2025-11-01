using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.WorkshopSessions.Commands;
using Wokiwoki.Application.Features.WorkshopSessions.Queries;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Api.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class WorkshopSessionController : ControllerBase
    {
        private readonly IMediator _mediator;


        public WorkshopSessionController(IMediator mediator)
        {
            _mediator = mediator;
        }

		[HttpGet("schedule/{scheduleId}/sessions/week")]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Get weekly sessions by schedule",
			Description = "Retrieves all workshop sessions within a specific date range (usually a week) for a given schedule.",
			Tags = new[] { "Workshop Session" }
		)]
		[SwaggerResponse(StatusCodes.Status200OK, "Sessions retrieved successfully", typeof(List<WorkshopSessionDto>))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request parameters")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "No sessions found for the given schedule")]
		public async Task<IActionResult> GetSessionsWeekByScheduleId(
			Guid scheduleId,
			[FromQuery] DateTime? startTime,
			[FromQuery] DateTime? endTime)
		{
			if (scheduleId == Guid.Empty)
				return BadRequest("ScheduleId is required.");

			var query = new GetSessionsWeekByScheduleIdQuery(scheduleId, startTime, endTime);
			var result = await _mediator.Send(query);

			//if (result == null || result.Count == 0)
			//	return NotFound("No sessions found for this schedule in the given date range.");

			return Ok(result);
		}

		/// <summary>
		/// Retrieve all sessions belonging to a specific schedule.
		/// </summary>
		/// <remarks>
		/// Returns a list of workshop sessions associated with the given schedule ID.
		/// </remarks>
		[HttpGet("schedules/{scheduleId:guid}/sessions")]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Get sessions by schedule ID",
			Description = "Fetches all workshop sessions that belong to the specified schedule.",
			Tags = new[] { "Workshop Session" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Sessions retrieved successfully", typeof(List<WorkshopSessionDto>))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid or missing Schedule ID")]
		public async Task<IActionResult> GetSessionsByScheduleId(Guid scheduleId)
		{
			if (scheduleId == Guid.Empty)
				return BadRequest("Schedule ID cannot be empty.");

			var query = new GetSessionByScheduleIdQuery(scheduleId);
			var result = await _mediator.Send(query);

			return Ok(result);
		}

		[HttpGet("{id:guid}")]
		[Produces("application/json")]
		[SwaggerOperation(
		Summary = "Get session by ID",
		Description = "Retrieves detailed information of a single session by its unique ID.",
		Tags = new[] { "Workshop Session" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Session found successfully", typeof(WorkshopSessionDto))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Session not found")]
		public async Task<IActionResult> GetById(Guid id)
		{
			if (id == Guid.Empty)
				return BadRequest("Invalid ID.");

			var result = await _mediator.Send(new GetSessionByIdQuery(id));
			if (result == null)
				return NotFound();

			return Ok(result);
		}

		[HttpPost]
		[Produces("application/json")]
		[SwaggerOperation(
		Summary = "Create new session",
		Description = "Creates a new workshop session record.",
		Tags = new[] { "Workshop Session" })]
		[SwaggerResponse(StatusCodes.Status201Created, "Session created successfully", typeof(Guid))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request body")]
		public async Task<IActionResult> Create([FromBody] CreateSessionCommand command)
		{
			if (command == null)
				return BadRequest("Invalid request body.");

			var result = await _mediator.Send(command);
			return CreatedAtAction(nameof(GetById), new { id = result.Id }, new { id = result.Id });
		}



		[HttpPut]
		[Produces("application/json")]
		[SwaggerOperation(
		Summary = "Update existing session",
		Description = "Updates an existing session by its ID.",
		Tags = new[] { "Workshop Session" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Session updated successfully")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid ID or request body")]
		public async Task<IActionResult> Update([FromBody] UpdateSessionCommand command)
		{
			if (command.Id == Guid.Empty)
				return BadRequest("Session ID is required.");

			var result = await _mediator.Send(command);
			return Ok(result);
		}


		[HttpPost("auto-generate")]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Auto-generate 1-month sessions",
			Description = "Automatically generates workshop sessions for one month based on a schedule.",
			Tags = new[] { "Workshop Session" }
		)]
		[ProducesResponseType(typeof(List<Guid>), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Create1MonthSessions([FromBody] Create1MonthSessionCommand command)
		{
			if (command is null)
				return BadRequest("Request body cannot be null.");

			if (command.scheduleId == Guid.Empty)
				return BadRequest("Schedule ID is required.");

			var result = await _mediator.Send(command);

			if (result is null || result.Count == 0)
				return NotFound("No sessions were generated.");

			return CreatedAtAction(
				nameof(Create1MonthSessions),
				new { scheduleId = command.scheduleId },
				result
			);
		}


	}
}