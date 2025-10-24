using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Application.Features.UserWorkShopLike.Commands.CreateUserWorkShopLike;
using Wokiwoki.Application.Features.UserWorkShopLike.Commands.DeleteUserWorkShopLike;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserWorkshopLikesController : ControllerBase
	{
		private readonly IMediator _mediator;

		public UserWorkshopLikesController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Like a workshop.
		/// </summary>
		/// <remarks>
		/// Allows an authenticated user to like a workshop by its ID.
		/// The user must be authenticated with a valid JWT token containing a UserId claim.
		/// If the user has already liked the workshop, the request will be ignored.
		/// Increments the workshop's like count and creates a new like record.
		/// </remarks>
		/// <param name="workshopId">The ID of the workshop to like.</param>
		/// <returns>A 201 Created response with the ID of the new like record.</returns>
		[Authorize(Policy = "RequireCustomerRole")]
		[HttpPost("{workshopId:guid}")]
		[SwaggerOperation(
			Summary = "Like a workshop",
			Description = "Allows an authenticated user to like a workshop by its ID. Requires a valid JWT token with a UserId claim.",
			Tags = new[] { "Workshop Likes" })]
		[SwaggerResponse(StatusCodes.Status201Created, "Workshop liked successfully", typeof(Guid))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid workshop ID")]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authenticated")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Workshop not found")]
		public async Task<IActionResult> LikeWorkshop(Guid workshopId)
		{
			try
			{
				var command = new CreateUserWorkshopLikeCommand(workshopId);
				var result = await _mediator.Send(command);
				return CreatedAtAction(nameof(LikeWorkshop), new { workshopId }, new { id = result });
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { Message = ex.Message });
			}
			catch (UnauthorizedAccessException)
			{
				return Unauthorized(new { Message = "User is not authenticated." });
			}
		}

		/// <summary>
		/// Unlike a workshop.
		/// </summary>
		/// <remarks>
		/// Allows an authenticated user to remove their like from a workshop by its ID.
		/// The user must be authenticated with a valid JWT token containing a UserId claim.
		/// If the user has not liked the workshop, the request will be ignored.
		/// Decrements the workshop's like count and removes the like record.
		/// </remarks>
		/// <param name="workshopId">The ID of the workshop to unlike.</param>
		/// <returns>A 200 OK response if the like is removed successfully.</returns>
		[Authorize(Policy = "RequireCustomerRole")]
		[HttpDelete("{workshopId:guid}")]
		[SwaggerOperation(
			Summary = "Unlike a workshop",
			Description = "Removes the authenticated user's like from a workshop by its ID. Requires a valid JWT token with a UserId claim.",
			Tags = new[] { "Workshop Likes" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Workshop unliked successfully")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid workshop ID")]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authenticated")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Workshop or like not found")]
		public async Task<IActionResult> UnlikeWorkshop(Guid workshopId)
		{
			try
			{
				var command = new DeleteUserWorkshopLikeCommand(workshopId);
				await _mediator.Send(command);
				return Ok(new { Message = "Workshop unliked successfully." });
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { Message = ex.Message });
			}
			catch (UnauthorizedAccessException)
			{
				return Unauthorized(new { Message = "User is not authenticated." });
			}
		}

	}
}
