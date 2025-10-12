using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Auth.Commands.EmailLogin;
using Wokiwoki.Application.Features.Auth.Commands.EmailRegistration.RegisterUserWithEmail;
using Wokiwoki.Application.Features.Auth.Commands.EmailRegistration.SendEmailVerificationCode;
using Wokiwoki.Application.Features.Auth.Commands.EmailRegistration.VerifyEmailCode;
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
		[HttpPost("send-email-code")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Send verification code to email",
			Description = "Send an OTP/verification code to the provided email address. Used for registration or password reset.",
			Tags = new[] { "Auth" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Code sent successfully", typeof(Result))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid email or malformed request")]
		public async Task<ActionResult<Result>> SendEmailCode([FromBody] SendEmailVerificationCodeCommand command)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await _mediator.Send(command);
			return Ok(result);
		}

		/// <summary>
		/// Verify the verification code sent to the email.
		/// </summary>
		[HttpPost("verify-email-code")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Verify email verification code",
			Description = "Validate the verification code sent to the email to complete the verification step.",
			Tags = new[] { "Auth" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Verification successful", typeof(Result))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid code or request data")]
		public async Task<ActionResult<Result>> VerifyEmailCode([FromBody] VerifyEmailCodeCommand command)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await _mediator.Send(command);
			return Ok(result);
		}

		/// <summary>
		/// Register a new account using an email address.
		/// </summary>
		[HttpPost("register-email")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Register with email",
			Description = "Create a new user account. Returns authentication info (access & refresh tokens and user info).",
			Tags = new[] { "Auth" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Registration successful", typeof(AuthDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
		public async Task<ActionResult<AuthDto>> RegisterWithEmail([FromBody] RegisterUserWithEmailCommand command)
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


	}
} 