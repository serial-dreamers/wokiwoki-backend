using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Tickets.Commands.CheckInTicket;
using Wokiwoki.Application.Features.Tickets.Queries.GetOrganizerParticipants;
using Wokiwoki.Application.Features.Tickets.Queries.GetTicketById;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TicketsController : ControllerBase
	{
		private readonly IMediator _mediator;
		public TicketsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("{id}")]
		[Authorize]
		[SwaggerOperation(
			Summary = "Get ticket details by ID",
			Description = "Retrieves detailed information about a specific ticket including booking, workshop, and session details.",
			Tags = new[] { "Tickets" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Ticket retrieved successfully", typeof(TicketDto))]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Ticket not found")]
		[SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while retrieving ticket")]
		public async Task<IActionResult> GetTicketById(Guid id)
		{
			try
			{
				var result = await _mediator.Send(new GetTicketByIdQuery(id));
				return Ok(result);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		/// <summary>
		/// Get participants for organizer's workshops with filtering and paging
		/// </summary>
		[Authorize]
		[HttpGet("organizer/participants")]
		[Authorize]
		[SwaggerOperation(
			Summary = "Get participants for organizer's workshops",
			Description = "Retrieves paginated list of participants (tickets) for workshops owned by the authenticated organizer. Supports filtering by workshop, session, date, check-in status, and search term.",
			Tags = new[] { "Tickets", "Organizer" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Participants retrieved successfully", typeof(PaginatedList<ParticipantDto>))]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated")]
		[SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while retrieving participants")]
		public async Task<IActionResult> GetOrganizerParticipants(
			[FromQuery] Guid? workshopId = null,
			[FromQuery] Guid? sessionId = null,
			[FromQuery] DateTime? startDate = null,
			[FromQuery] DateTime? endDate = null,
			[FromQuery] bool? isCheckedIn = null,
			[FromQuery] string? searchTerm = null,
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 20)
		{
			try
			{
				var query = new GetOrganizerParticipantsQuery(
					workshopId,
					sessionId,
					startDate,
					endDate,
					isCheckedIn,
					searchTerm,
					pageNumber,
					pageSize
				);

				var result = await _mediator.Send(query);
				return Ok(result);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		/// <summary>
		/// Check-in a ticket
		/// </summary>
		[Authorize]		
		[HttpPost("check-in/{ticketId}")]
		[Authorize]
		[SwaggerOperation(
			Summary = "Check-in a ticket",
			Description = "Marks a ticket as checked-in. Only the organizer who owns the workshop can check-in tickets.",
			Tags = new[] { "Tickets", "Organizer" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Ticket checked-in successfully", typeof(Result))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Ticket already checked-in or invalid")]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated")]
		[SwaggerResponse(StatusCodes.Status403Forbidden, "User not authorized to check-in this ticket")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Ticket not found")]
		[SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while checking-in ticket")]
		public async Task<IActionResult> CheckInTicket(Guid ticketId)
		{
			try
			{
				var command = new CheckInTicketCommand(ticketId);
				var result = await _mediator.Send(command);

				if (!result.Succeeded)
					return BadRequest(result);

				return Ok(result);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}
	}
}
