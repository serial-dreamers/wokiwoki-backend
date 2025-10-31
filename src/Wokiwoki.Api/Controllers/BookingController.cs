using MediatR;
using Microsoft.AspNetCore.Mvc;
using Wokiwoki.Application.Features.Bookings.Commands;
using Wokiwoki.Application.Features.Bookings.Queries;

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

        [HttpPatch("UpdateStatus/")]
        /// <summary>
        /// Update booking status
        /// </summary>
        public async Task<IActionResult> UpdateBookingStatus(UpdateBookingStatusCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPatch("Confirm/")]
        /// <summary>
        /// Confirm booking
        /// </summary>
        public async Task<IActionResult> ConfirmBooking(ConfirmBookingCommand command)
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            var result = await _mediator.Send(command with { Authorization = authHeader });
            return Ok(result);
        }


        /// <summary>
        /// Get booking by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound("Invalid booking ID.");
            }

            var result = await _mediator.Send(new GetBookingByIdQuery(id));

            if (result == null)
            {
                return NotFound("Booking not found.");
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a new booking
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingCommand command)
        {
            if (command == null)
            {
                return BadRequest("Booking data is required.");
            }

            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
    }
}
