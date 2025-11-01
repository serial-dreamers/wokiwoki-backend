using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.ScheduleTickets.Command;
using Wokiwoki.Application.Features.ScheduleTickets.Queries.GetScheduleById;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Api.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]

    public class WorkshopScheduleTicketController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkshopScheduleTicketController(IMediator mediator)
        {
            _mediator = mediator;
        }

		/// <summary>
		/// Create a new schedule ticket.
		/// </summary>
		/// <remarks>
		/// Creates a new ticket type (e.g. Early Bird, Regular) for a specific workshop schedule.
		/// </remarks>
		/// <param name="command">Ticket creation details including schedule ID, name, price, and quantity.</param>
		/// <returns>Returns the ID of the created ticket.</returns>
		[HttpPost]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Create a schedule ticket",
			Description = "Creates a new ticket under a specific workshop schedule.",
			Tags = new[] { "Schedule Tickets" })]
		[SwaggerResponse(StatusCodes.Status201Created, "Ticket created successfully", typeof(object))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request or failed to create ticket")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Workshop schedule not found")]
		[SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
		public async Task<IActionResult> CreateScheduleTicket([FromBody] CreateScheduleTicketCommand command)
		{
			if (command == null)
				return BadRequest(new { message = "Request body is null." });

			var result = await _mediator.Send(command);

			if (!result.Succeeded)
			{
				if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
					return NotFound(new { errors = result.Errors });

				return BadRequest(new { errors = result.Errors });
			}

			return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { id = result.Value });
			 
		}

		/// <summary>
		/// Get a schedule ticket by ID.
		/// </summary>
		/// <remarks>
		/// Retrieves the details of a specific workshop schedule ticket by its ID.
		/// </remarks>
		/// <param name="id">The unique ID of the schedule ticket.</param>
		/// <returns>Details of the ticket.</returns>
		[HttpGet("{id:guid}")]
		[SwaggerOperation(
			Summary = "Get schedule ticket by ID",
			Description = "Fetch details of a specific workshop schedule ticket.",
			Tags = new[] { "Schedule Tickets" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Ticket found", typeof(WorkshopScheduleTicketDto))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Ticket not found")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var ticket = await _mediator.Send(new GetScheduleTicketByIdQuery(id));

			if (ticket == null)
				return NotFound(new { message = "Schedule ticket not found." });

			return Ok(ticket);
		}

	}
}
