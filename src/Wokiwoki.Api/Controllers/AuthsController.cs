using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Auth.Commands.ChangePassword;
using Wokiwoki.Application.Features.Auth.Commands.EmailLogin;
using Wokiwoki.Application.Features.Auth.Commands.EmailRegistration.RegisterUserWithEmail;
using Wokiwoki.Application.Features.Auth.Commands.EmailRegistration.SendEmailVerificationCode;
using Wokiwoki.Application.Features.Auth.Commands.EmailRegistration.VerifyEmailCode;
using Wokiwoki.Application.Features.Auth.Commands.RefreshToken;
using Wokiwoki.Application.Features.Auth.Commands.ResetPassword;
using Wokiwoki.Application.Features.Auth.Commands.SendForgotPasswordCode;
using Wokiwoki.Application.Features.Auth.Commands.VerifyForgotPasswordCode;
using Wokiwoki.Application.Features.Users.Commands.GoogleLogin;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthsController : ControllerBase
	{
		private readonly ISender _mediator;

		public AuthsController(ISender mediator)
		{
			_mediator = mediator;
		}


		/// <summary>
		/// Send a verification code to an email address.
		/// </summary>
		/// <remarks>
		/// Use this endpoint during registration or password reset flows.
		/// </remarks>
		[HttpPost("register/send")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Send verification code to email",
			Description = "Send an OTP/verification code to the provided email address. Used for registration or password reset.",
			Tags = new[] { "Auth" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Code sent successfully", typeof(Result))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid email or malformed request")]
		public async Task<ActionResult<Result>> SendRegisterCode([FromBody] SendEmailVerificationCodeCommand command)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await _mediator.Send(command);

			if (!result.Succeeded)
				return BadRequest(result);

			return Ok(result);
		}

		/// <summary>
		/// Verify the verification code sent to the email.
		/// </summary>
		[HttpPost("register/verify")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Verify email verification code",
			Description = "Validate the verification code sent to the email to complete the verification step.",
			Tags = new[] { "Auth" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Verification successful", typeof(Result))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid code or request data")]
		public async Task<ActionResult<Result>> VerifyRegisterCode([FromBody] VerifyEmailCodeCommand command)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await _mediator.Send(command);

			if (!result.Succeeded)
				return BadRequest(result);

			return Ok(result);
		}

		/// <summary>
		/// Register a new account using an email address.
		/// </summary>
		[HttpPost("register")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Register with email",
			Description = "Create a new user account. Returns authentication info (access & refresh tokens and user info).",
			Tags = new[] { "Auth" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Registration successful", typeof(AuthDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request or unverified code")]
		[SwaggerResponse(StatusCodes.Status409Conflict, "Email already registered")]
		public async Task<ActionResult<AuthDto>> CompleteRegisterWithEmail([FromBody] RegisterUserWithEmailCommand command)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var auth = await _mediator.Send(command);

			return Ok(auth);
		}

		/// <summary>
		/// User login with email and password.
		/// </summary>
		[HttpPost("login")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Login with email",
			Description = "Returns AccessToken, RefreshToken and user information upon successful login.",
			Tags = new[] { "Auth" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Login successful", typeof(AuthDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid credentials")]
		public async Task<ActionResult<AuthDto>> Login([FromBody] EmailLoginCommand command)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var response = await _mediator.Send(command);

			// Assuming response contains .Result.Succeeded or similar
			if (response == null || (response.Result != null && !response.Result.Succeeded))
				return Unauthorized(response);

			return Ok(response);
		}

		/// <summary>
		/// Login with Google using a tokenId from the client.
		/// </summary>
		[HttpPost("google")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Login with Google",
			Description = "Accepts a tokenId from the client, validates it and returns AuthDto if successful.",
			Tags = new[] { "Auth" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Google login successful", typeof(AuthDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Missing or invalid Google token")]
		public async Task<ActionResult<AuthDto>> GoogleLogin([FromBody] GoogleLoginCommand command)
		{
			if (command == null || string.IsNullOrEmpty(command.tokenId))
				return BadRequest(new { message = "Missing Google tokenId" });

			var result = await _mediator.Send(command);

			if (result == null || (result.Result != null && !result.Result.Succeeded))
				return BadRequest(result?.Message ?? "Google login failed");

			return Ok(result);
		}

		/// <summary>
		/// Rotate refresh token to get a new access token and refresh token.
		/// </summary>
		[HttpPost("refresh-token")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Rotate refresh token",
			Description = "Returns new AccessToken and RefreshToken upon successful rotation.",
			Tags = new[] { "Auth" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Rotation successful", typeof(RotateRefreshTokenDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid model")]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid or expired refresh token")]
		public async Task<IActionResult> Rotate([FromBody] RotateRefreshTokenCommand command)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				var response = await _mediator.Send(command);
				return Ok(response);
			}
			catch (SecurityTokenException ex)
			{
				return Unauthorized(new { error = ex.Message });
			}
			catch (ValidationException ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}


		/// <summary>
		/// Change the password of the currently logged-in user.
		/// </summary> 
		[HttpPost("change-password")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Change password",
			Description = "Change the current password of a logged-in user. Requires authentication.",
			Tags = new[] { "Auth" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Password changed successfully")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request or incorrect current password")]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized - user must be logged in")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
		{   
			var result = await _mediator.Send(command);

			if (!result.Succeeded)
				return BadRequest(result.Errors);

			return Ok(new { message = "Password changed successfully" });
		}


		/// <summary>
		/// Send a password reset verification code to email.
		/// </summary>
		[HttpPost("forgot-password/send")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Send password reset code",
			Description = "Sends an OTP code to the provided email for password reset.",
			Tags = new[] { "Auth" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Code sent successfully", typeof(Result))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid email")]
		public async Task<ActionResult<Result>> SendForgotPasswordCode([FromBody] SendForgotPasswordCodeCommand command)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await _mediator.Send(command);
			return Ok(result);
		}

		/// <summary>
		/// Verify the password reset code sent to the email.
		/// </summary>
		[HttpPost("forgot-password/verify")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Verify password reset code",
			Description = "Validate the OTP sent to email to continue password reset process.",
			Tags = new[] { "Auth" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Verification successful", typeof(Result))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid or expired code")]
		public async Task<ActionResult<Result>> VerifyForgotPasswordCode([FromBody] VerifyForgotPasswordCodeCommand command)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await _mediator.Send(command);
			return Ok(result);
		}

		/// <summary>
		/// Reset password after verifying email.
		/// </summary>
		[HttpPost("forgot-password/reset")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Reset password",
			Description = "Reset user password after successful code verification.",
			Tags = new[] { "Auth" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Password reset successful", typeof(Result))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request or unverified email")]
		public async Task<ActionResult<Result>> ResetPassword([FromBody] ResetPasswordCommand command)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await _mediator.Send(command);
			return Ok(result);
		}
	}
} 