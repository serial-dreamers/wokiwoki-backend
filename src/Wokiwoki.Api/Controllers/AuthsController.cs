using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.X86;
using Wokiwoki.Application.Features.Users.Commands;
using Wokiwoki.Application.Features.Users.Commands.EmailRegistration.RegisterUserWithEmail;
using Wokiwoki.Application.Features.Users.Commands.EmailRegistration.SendEmailVerificationCode;
using Wokiwoki.Application.Features.Users.Commands.EmailRegistration.VerifyEmailCode;
using Wokiwoki.Application.Features.Users.Commands.UsernameLogin;

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

		[HttpPost("send-email-code")]
		public async Task<IActionResult> SendEmailCode([FromBody] SendEmailVerificationCodeCommand command)
		=> Ok(await _mediator.Send(command));

		[HttpPost("verify-email-code")]
		public async Task<IActionResult> VerifyEmailCode([FromBody] VerifyEmailCodeCommand command)
		=> Ok(await _mediator.Send(command));

		[HttpPost("register-email")]
		public async Task<IActionResult> RegisterWithEmail([FromBody] RegisterUserWithEmailCommand command)
		=> Ok(await _mediator.Send(command));

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] UsernameLoginCommand command)
		{  
			var response = await _mediator.Send(command);

			if (!response.Result.Succeeded)
				return Unauthorized(response);   

			return Ok(response); 
		}
	}
}
//User Id = postgres.xofwjbyptdhgubfcyjmy; Password = Khanh@123Woki; Server = aws - 1 - ap - southeast - 1.pooler.supabase.com; Port = 6543; Database = postgres
//Host = db.xofwjbyptdhgubfcyjmy.supabase.co; Database = postgres; Username = postgres; Password = Khanh@123Woki; SSL Mode = Require; Trust Server Certificate=true
 