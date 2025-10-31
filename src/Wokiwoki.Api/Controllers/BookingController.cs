using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.Features.Bookings.Commands;
using Wokiwoki.Application.Features.Bookings.Queries;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingController(IMediator mediator)
        {
            _mediator = mediator;
        }

		/// <summary>
		/// Get booking by ID.
		/// </summary>
		[HttpGet("{id}")]
		[SwaggerOperation(
			Summary = "Get booking by ID",
			Description = "Retrieves booking details by its unique identifier.",
			Tags = new[] { "Booking" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Booking retrieved successfully", typeof(Booking))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Booking not found")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid booking ID")]
		[SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while retrieving booking")]
		public async Task<IActionResult> GetById(Guid id)
		{
			if (id == Guid.Empty)
				return BadRequest("Invalid booking ID.");

			var result = await _mediator.Send(new GetBookingByIdQuery(id));

			if (result == null)
				return NotFound("Booking not found.");

			return Ok(result);
		}

		/// <summary>
		/// Create a new booking.
		/// </summary>
		[HttpPost]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Create new booking",
			Description = "Creates a new booking for a specific workshop or event.",
			Tags = new[] { "Booking" })]
		[SwaggerResponse(StatusCodes.Status201Created, "Booking created successfully", typeof(Booking))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid booking data or missing fields")]
		[SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while creating booking")]
		public async Task<IActionResult> Create([FromBody] CreateBookingCommand command)
		{
			if (command == null)
				return BadRequest("Booking data is required.");

			var result = await _mediator.Send(command);
			return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
		}

		/// <summary>
		/// Confirm booking.
		/// </summary>
		[HttpPatch("confirm")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Confirm booking",
			Description = "Confirms a booking and triggers necessary validation or payment checks. Requires Authorization header.",
			Tags = new[] { "Booking" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Booking confirmed successfully", typeof(Result))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid booking data or request")]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "Authorization header is missing or invalid")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Booking not found")]
		[SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while confirming booking")]
		public async Task<IActionResult> ConfirmBooking([FromBody] ConfirmBookingCommand command)
		{
			var authHeader = Request.Headers["Authorization"].ToString();

			if (string.IsNullOrEmpty(authHeader))
				return Unauthorized("Authorization header is required.");

			var result = await _mediator.Send(command with { Authorization = authHeader });
			return Ok(result);
		}

		/// <summary>
		/// Update booking status.
		/// </summary>
		[HttpPatch("update-status")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Update booking status",
			Description = "Updates the status of a booking (e.g., pending, confirmed, cancelled).",
			Tags = new[] { "Booking" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Booking status updated successfully", typeof(Result))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid booking status or request data")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Booking not found")]
		[SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while updating booking status")]
		public async Task<IActionResult> UpdateBookingStatus([FromBody] UpdateBookingStatusCommand command)
		{
			var result = await _mediator.Send(command);
			return Ok(result);
		}

	}
}
