using MediatR;
using Microsoft.AspNetCore.Mvc;
using Wokiwoki.Application.Features.Sepay.QRCode;

namespace Wokiwoki.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SepayController : Controller
    {
        private readonly IMediator _mediator;

        public SepayController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        ///<summary>
        ///Generate QR Code for check out   
        /// </summary>
        public async Task<IActionResult> GenerateQRCode([FromBody] GenerateQRCodeCommand command)
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            // Gửi header xuống handler
            var result = await _mediator.Send(command with { Authorization = authHeader });
            return Ok(result);
        }

    }
}
