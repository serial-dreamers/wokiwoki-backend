using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Api.Request;
using Wokiwoki.Application.DTOs;
using Wokiwoki.Application.Features.Chatbots.Commands.CreateChatAIMessage;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ChatsController : ControllerBase
	{
		private readonly IMediator _mediator;

		public ChatsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Create a new AI chat message and get response.
		/// </summary>
		/// <remarks>
		/// Sends the user's message to the Azure OpenAI chat service, maintaining conversation history
		/// via the user's active conversation in the database. Returns the AI's Markdown-formatted response.
		/// UserId is used to load/save conversation context.
		/// </remarks>
		/// <param name="request">The chat request containing the message and user ID.</param>
		[HttpPost("messages")]
		[ProducesResponseType(typeof(ChatResponse), StatusCodes.Status200OK)]
		[SwaggerOperation(
			Summary = "Send AI chat message",
			Description = "Processes a user message through the AI chat service and returns the response.",
			Tags = new[] { "Chat" })]
		[SwaggerResponse(StatusCodes.Status200OK, "AI response returned successfully")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request (e.g., empty message)")]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated")]
		[SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error during AI processing")]
		public async Task<ActionResult<ChatResponse>> CreateMessage([FromBody] CreateChatRequest request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(request.UserMessage))
			{
				return BadRequest("User message cannot be empty.");
			}

			var command = new CreateChatAIMessageCommand(request.UserMessage, request.UserId);
			var response = await _mediator.Send(command, cancellationToken);

			return Ok(response);
		}

}
}
