using MediatR;
using Microsoft.AspNetCore.Mvc;
using Wokiwoki.Application.Features.ScheduleTickets.Command;

namespace Wokiwoki.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class WorkshopScheduleTicketController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkshopScheduleTicketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        ///<summary>
        ///Create Schedule ticket
        /// </summary>
        public async Task<IActionResult> CreateScheduleTicket([FromBody] CreateScheduleTicketCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        } 

    }
}
