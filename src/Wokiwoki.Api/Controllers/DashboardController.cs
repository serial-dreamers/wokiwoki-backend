using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Dashboard.Queries.GetOrganizerDashboard;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DashboardController : ControllerBase
	{
		private readonly IMediator _mediator;

		public DashboardController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Get dashboard statistics for organizer
		/// </summary>
		[HttpGet("organizer")]
		[Authorize]
		[SwaggerOperation(
			Summary = "Get organizer dashboard statistics",
			Description = "Retrieves comprehensive dashboard statistics for the authenticated organizer including revenue, tickets sold, workshops, and charts. Supports filtering by date range, workshop, and chart grouping (day/week/month).",
			Tags = new[] { "Dashboard", "Organizer" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Dashboard data retrieved successfully", typeof(OrganizerDashboardDto))]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated")]
		[SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while retrieving dashboard")]
		public async Task<IActionResult> GetOrganizerDashboard(
			[FromQuery] DateTime? startDate = null,
			[FromQuery] DateTime? endDate = null,
			[FromQuery] Guid? workshopId = null,
			[FromQuery] string chartGroupBy = "day")
		{
			try
			{
				// Validate chartGroupBy
				if (!new[] { "day", "week", "month" }.Contains(chartGroupBy.ToLower()))
				{
					return BadRequest("chartGroupBy must be 'day', 'week', or 'month'");
				}

				var query = new GetOrganizerDashboardQuery(
					startDate,
					endDate,
					workshopId,
					chartGroupBy
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
	}
}

