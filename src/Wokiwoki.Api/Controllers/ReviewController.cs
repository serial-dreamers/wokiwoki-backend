using MediatR;
using Microsoft.AspNetCore.Mvc;
using Wokiwoki.Api.Request;
using Wokiwoki.Application.Features.Reviews.Command;
using Wokiwoki.Application.Features.WorkshopMedias.Commands.CreateWorkshopMedia;

namespace Wokiwoki.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReviewController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReviewRequest request)
        {
            var command = new CreateReviewCommand(
                request.WorkshopId,
                request.BookingId,
                request.LogoFile.FileName,
                request.LogoFile.ContentType,
                request.LogoFile.Length,
                request.LogoFile.OpenReadStream(),
                request.UserId,
                request.Comment,
                request.Rating
            );
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
