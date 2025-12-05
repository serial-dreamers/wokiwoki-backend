using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
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
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get booking by time, organizer?, category?, tag? with paging",
            Tags = new[] { "Booking" })]
        public async Task<IActionResult> GetBookingFilter([FromQuery] GetBookingFilterQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("by-time")]
        [SwaggerOperation(
            Summary = "Get booking by time",
            Tags = new[] { "Booking" })]
        public async Task<IActionResult> GetBookingByTIme([FromQuery] GetBookingByTimeQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
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

        [HttpPost("confirm")]
        [Consumes("application/json")]
        [SwaggerOperation(
    Summary = "Confirm booking (Webhook from Sepay)",
    Description = "Sepay sends payment notification here. Confirms a booking and triggers necessary validation or payment checks.",
    Tags = new[] { "Booking" })]
        [SwaggerResponse(StatusCodes.Status200OK, "Booking confirmed successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid booking data or request")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authorization header is missing or invalid")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Booking not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while confirming booking")]
        public async Task<IActionResult> ConfirmBooking([FromBody] JsonElement payload)
        {

            // ✅ Lấy content từ body
            string? content = payload.TryGetProperty("content", out var c) ? c.GetString() : null;

            // ✅ Lấy Authorization header
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader) || string.IsNullOrWhiteSpace(authHeader))
                return Unauthorized(new { message = "Authorization header is required." });

            // ✅ Gửi command qua Mediator
            var command = new ConfirmBookingCommand(
                Content: content ?? string.Empty,
                Authorization: authHeader
            );

            var isConfirmed = await _mediator.Send(command);

            return Ok(isConfirmed);

        }

        ///// <summary>
        ///// Confirm booking (Webhook from Sepay)
        ///// </summary>
        //[HttpPost("confirm")]
        //[Consumes("application/json")]
        //[SwaggerOperation(
        //    Summary = "Confirm booking (Webhook from Sepay)",
        //    Description = "Sepay sends payment notification here. Confirms a booking and triggers necessary validation or payment checks.",
        //    Tags = new[] { "Booking" })]
        //[SwaggerResponse(StatusCodes.Status200OK, "Booking confirmed successfully")]
        //[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid booking data or request")]
        //[SwaggerResponse(StatusCodes.Status401Unauthorized, "Authorization header is missing or invalid")]
        //[SwaggerResponse(StatusCodes.Status404NotFound, "Booking not found")]
        //[SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while confirming booking")]
        //public async Task<IActionResult> ConfirmBooking([FromBody] JsonElement payload)
        //{
        //    try
        //    {
        //        string? content = payload.GetProperty("content").GetString();

        //        var command = new ConfirmBookingCommand(
        //            Content: content ?? string.Empty,
        //            Authorization: Request.Headers["Authorization"].ToString()
        //        );
        //        // 1️⃣ Kiểm tra Authorization header
        //        if (!Request.Headers.TryGetValue("Authorization", out var authHeader) || string.IsNullOrWhiteSpace(authHeader))
        //            return Unauthorized(new { message = "Authorization header is required." });

        //        // 2️⃣ Gửi command đến handler, có Authorization
        //        var isConfirmed = await _mediator.Send(command with { Authorization = authHeader });

        //        // 3️⃣ Mapping kết quả
        //        if (!isConfirmed)
        //            return BadRequest(new { message = "Failed to confirm booking. Please verify API key or BookingId." });

        //        // ✅ Thành công
        //        return Ok(new { message = "Booking confirmed successfully." });
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return NotFound(new { message = "Booking not found." });
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        return Unauthorized(new { message = "You are not authorized to confirm this booking." });
        //    }
        //    catch (Exception ex)
        //    {
        //        // ⚠️ Lỗi không mong đợi
        //        return StatusCode(StatusCodes.Status500InternalServerError, new
        //        {
        //            message = "An unexpected error occurred while confirming booking.",
        //            error = ex.Message
        //        });
        //    }
        //}

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
