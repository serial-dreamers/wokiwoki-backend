using MediatR;
using Microsoft.AspNetCore.Mvc;
using Wokiwoki.Application.Features.WorkshopSessions.Commands;
using Wokiwoki.Application.Features.WorkshopSessions.Queries;

namespace Wokiwoki.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkshopSessionController : ControllerBase
    {
        private readonly IMediator _mediator;


        public WorkshopSessionController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("auto-generate/{scheduleId:guid}")]
        /// <summary>
        /// Generate workshop sessions automatically for 1 month based on the schedule.
        /// </summary>
        public async Task<IActionResult> Create1MonthSessions([FromBody] Create1MonthSessionCommand command)
        {
            if (command == null)
                return BadRequest("Invalid request body.");

            if (command.scheduleId == Guid.Empty)
                return BadRequest("ScheduleId is required.");

            var result = await _mediator.Send(command);

            if (result == null || result.Count == 0)
                return NotFound("No sessions generated.");

            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            var result = await _mediator.Send(new GetSessionByIdQuery(id));
            return Ok(result);
        }
        [HttpPost]
        ///<summary>
        ///Create
        /// </summary>
        public async Task<IActionResult> Create(CreateSessionCommand command)
        {
            if (command == null)
            {
                return NotFound();
            }
            var result = await _mediator.Send(command);
			return CreatedAtAction(nameof(GetById), new { id = result.Id }, new { result });

        }
    }
}