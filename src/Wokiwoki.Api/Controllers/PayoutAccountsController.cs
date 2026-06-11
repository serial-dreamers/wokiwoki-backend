using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.OrganizationPayoutAccounts.Commands.CreatePayoutAccount;
using Wokiwoki.Application.Features.OrganizationPayoutAccounts.Commands.DeletePayoutAccount;
using Wokiwoki.Application.Features.OrganizationPayoutAccounts.Commands.UpdatePayoutAccount;
using Wokiwoki.Application.Features.OrganizationPayoutAccounts.Queries.GetMyPayoutAccounts;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class PayoutAccountsController : ControllerBase
	{
		private readonly IMediator _mediator;

		public PayoutAccountsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Get my payout accounts
		/// </summary>
		[HttpGet("my-accounts")]
		[SwaggerOperation(
			Summary = "Get my payout accounts",
			Description = "Get all payout accounts for the authenticated organization.",
			Tags = new[] { "Payout Accounts" })]
		[ProducesResponseType(typeof(List<OrganizationPayoutAccountDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GetMyPayoutAccounts()
		{
			try
			{
				var result = await _mediator.Send(new GetMyPayoutAccountsQuery());
				return Ok(result);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(ex.Message);
			}
		}

		/// <summary>
		/// Create payout account
		/// </summary>
		[HttpPost]
		[SwaggerOperation(
			Summary = "Create payout account",
			Description = "Create a new payout account for the organization.",
			Tags = new[] { "Payout Accounts" })]
		[ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> CreatePayoutAccount([FromBody] CreatePayoutAccountCommand command)
		{
			try
			{
				var id = await _mediator.Send(command);
				return CreatedAtAction(nameof(GetMyPayoutAccounts), new { id }, new { id });
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		/// <summary>
		/// Update payout account
		/// </summary>
		[HttpPut("{id:guid}")]
		[SwaggerOperation(
			Summary = "Update payout account",
			Description = "Update an existing payout account.",
			Tags = new[] { "Payout Accounts" })]
		[ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> UpdatePayoutAccount(Guid id, [FromBody] UpdatePayoutAccountCommand command)
		{
			try
			{
				if (id != command.Id)
					return BadRequest(new { message = "ID mismatch" });

				var result = await _mediator.Send(command);
				if (!result.Succeeded)
					return BadRequest(result);

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		/// <summary>
		/// Delete payout account
		/// </summary>
		[HttpDelete("{id:guid}")]
		[SwaggerOperation(
			Summary = "Delete payout account",
			Description = "Delete a payout account.",
			Tags = new[] { "Payout Accounts" })]
		[ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeletePayoutAccount(Guid id)
		{
			try
			{
				var command = new DeletePayoutAccountCommand(id);
				var result = await _mediator.Send(command);
				
				if (!result.Succeeded)
					return BadRequest(result);

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}
	}
}

