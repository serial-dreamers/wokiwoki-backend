using MediatR;
using Microsoft.AspNetCore.Mvc;
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
		/// Tạo schedule.
		/// </summary>
		[HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateScheduleCommand command)
        {
            if (command == null)
            {
                return BadRequest("Request is null");
            }
            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result );
                //return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

		/// <summary>
		/// Get By Id
		/// </summary>
		[HttpGet("{id}")] 
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == null)
            {
                return BadRequest("Request is null");
            }
            try
            {
                var result = await _mediator.Send(new GetScheduleByIdQuery ( id ));
                if (result == null)
                {
                    return NotFound("Schedule not found");
                }
                return Ok(result);

                //return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

