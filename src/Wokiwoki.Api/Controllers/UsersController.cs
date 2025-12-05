using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Api.Request;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Users.Commands.SaveUserPreferences;
using Wokiwoki.Application.Features.Users.Commands.UpdateUserAvatar;
using Wokiwoki.Application.Features.Users.Commands.UpdateUserProfile;
using Wokiwoki.Application.Features.Users.Queries.GetUserById;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class UsersController : ControllerBase
	{
		private readonly ISender _mediator;

		public UsersController(ISender mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Get current user profile information
		/// </summary>
		[Authorize]		
		[HttpGet("profile")]
		[SwaggerOperation(
			Summary = "Get current user profile",
			Description = "Returns the profile information of the currently authenticated user.",
			Tags = new[] { "Users" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Profile retrieved successfully", typeof(UserDto))]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
		public async Task<ActionResult<UserDto>> GetUserProfile()
		{
			try
			{
				var query = new GetUserByIdQuery();
				var user = await _mediator.Send(query);
				return Ok(user);
			}
			catch (Exception ex)
			{
				return NotFound(new { message = ex.Message });
			}
		}

		/// <summary>
		/// Update current user profile information
		/// </summary>
		[Authorize]
		[HttpPut("profile")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Update user profile",
			Description = "Updates the profile information of the currently authenticated user.",
			Tags = new[] { "Users" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Profile updated successfully", typeof(Result))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data")]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated")]
		public async Task<ActionResult<Result>> UpdateUserProfile([FromBody] UpdateUserProfileCommand command)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _mediator.Send(command);

			if (!result.Succeeded)
				return BadRequest(result);

			return Ok(result);
		}

		/// <summary>
		/// Upload/Update user avatar
		/// </summary>
		[Authorize]
		[HttpPost("avatar")]
		[Consumes("multipart/form-data")]
		[SwaggerOperation(
			Summary = "Upload user avatar",
			Description = "Uploads or updates the avatar image for the currently authenticated user. Only accepts image files (jpg, jpeg, png, gif, webp).",
			Tags = new[] { "Users" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Avatar uploaded successfully", typeof(Result<string>))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid file or request data")]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated")]
		public async Task<ActionResult<Result<string>>> UploadAvatar([FromForm] UpdateUserAvatarRequest request)
		{
			if (request.AvatarFile == null || request.AvatarFile.Length == 0)
				return BadRequest(new { message = "Không có file nào được tải lên." });

			// Validate file type - chỉ nhận ảnh
			var ext = Path.GetExtension(request.AvatarFile.FileName).ToLowerInvariant();
			if (!new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }.Contains(ext))
			{
				return BadRequest(new { message = "Định dạng file không hợp lệ. Chỉ hỗ trợ ảnh (.jpg, .jpeg, .png, .gif, .webp)." });
			}

			// Validate file size (max 5MB)
			if (request.AvatarFile.Length > 5 * 1024 * 1024)
			{
				return BadRequest(new { message = "Kích thước file vượt quá giới hạn 5MB." });
			}

			var command = new UpdateUserAvatarCommand(
				request.AvatarFile.FileName,
				request.AvatarFile.ContentType,
				request.AvatarFile.Length,
				request.AvatarFile.OpenReadStream()
			);

			var result = await _mediator.Send(command);

			if (!result.Succeeded)
				return BadRequest(result);

			return Ok(result);
		}

		/// <summary>
		/// Save user preferences (location, categories, tags)
		/// </summary>
		[Authorize]
		[HttpPost("preferences")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Save user preferences",
			Description = "Saves user location and tag preferences for workshop recommendations.",
			Tags = new[] { "Users" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Preferences saved successfully", typeof(Result))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data")]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated")]
		public async Task<ActionResult<Result>> SaveUserPreferences([FromBody] SaveUserPreferencesCommand command)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _mediator.Send(command);

			if (!result.Succeeded)
				return BadRequest(result);

			return Ok(result);
		}
	}
}
