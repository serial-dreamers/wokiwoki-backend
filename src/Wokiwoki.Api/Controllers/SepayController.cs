using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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

		/// <summary>
		/// Generate a QR code for checkout payment.
		/// </summary>
		[HttpPost]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Generate payment QR code",
			Description = "Creates a QR code via Sepay payment gateway for checkout transactions. The generated QR code allows users to complete payment securely.",
			Tags = new[] { "Sepay" })]
		[SwaggerResponse(StatusCodes.Status200OK, "QR code generated successfully", typeof(string))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request or missing payment details")]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid authorization header")]
		[SwaggerResponse(StatusCodes.Status500InternalServerError, "Error occurred while generating QR code")]
		public async Task<IActionResult> GenerateQRCode([FromBody] GenerateQRCodeCommand command)
		{
			var authHeader = Request.Headers["Authorization"].ToString();
			if (string.IsNullOrEmpty(authHeader))
				return Unauthorized("Authorization header is required");

			var result = await _mediator.Send(command);
			return Ok(result);
		}

	}
}
