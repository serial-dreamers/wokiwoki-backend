using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.WorkshopSchedules.Commands.CreateSchedule;
using Wokiwoki.Application.Features.WorkshopSchedules.Queries.GetSchedule;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]s")]
	[ApiController]
	public class WorkshopScheduleController : ControllerBase
	{
		private readonly IMediator _mediator;

		public WorkshopScheduleController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Get all schedules of a specific workshop with pagination.
		/// </summary>
		[HttpGet("workshop/{workshopId:guid}")]
		[SwaggerOperation(
			Summary = "Get paginated schedules for a workshop",
			Description = "Retrieves all recurrence schedules (daily, weekly, monthly, etc.) belonging to a specific workshop with pagination support.",
			Tags = new[] { "Schedules" }
		)]
		[SwaggerResponse(StatusCodes.Status200OK, "Schedules retrieved successfully", typeof(PaginatedList<WorkshopScheduleDto>))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "No schedules found for the specified workshop")]
		[SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error while retrieving schedules")]
		public async Task<IActionResult> GetByWorkshopId(
			Guid workshopId,
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10)
		{
			var result = await _mediator.Send(new GetSchedulesByWorkshopIdQuery(workshopId, pageNumber, pageSize));

			if (result == null || !result.Records.Any())
				return NotFound("No schedules found for this workshop.");

			return Ok(result);
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
		[SwaggerResponse(StatusCodes.Status201Created, "Schedule created successfully", typeof(Result<Guid>))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request body or missing required fields")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Workshop not found")]
		[SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error while creating schedule")]
		public async Task<IActionResult> Create([FromBody] CreateScheduleCommand command)
		{
			if (command == null)
				return BadRequest("Request is null");

			var result = await _mediator.Send(command);
			if (!result.Succeeded)
			{
				var error = result.Errors.FirstOrDefault() ?? "Unknown error";
				 
				if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
					return NotFound(new { message = error });

				// Các lỗi khác (VD: validation, DB, ...)
				return BadRequest(new { message = error });
			}

			return CreatedAtAction(nameof(GetById), new { id = result.Value }, new
			{ 
				id = result.Value
			});

		}

	}
}

