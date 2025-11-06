using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Workshops.Commands.CreateWorkshop;
using Wokiwoki.Application.Features.Workshops.Queries.GetDiscoverWorkshops;
using Wokiwoki.Application.Features.Workshops.Queries.GetFilterPagedWorkshopsQuery;
using Wokiwoki.Application.Features.Workshops.Queries.GetWorkshop;
using Wokiwoki.Application.Features.Workshops.Queries.GetWorkshopSessionsByDateRange;
using Wokiwoki.Domain.Enums; 

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WorkshopsController : ControllerBase
	{
		private readonly IMediator _mediator;
		

		public WorkshopsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("discover")]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Get discover workshops",
			Description = "Retrieves featured workshops for the discover section with organization information. Returns up to the specified limit of workshops. Supports filtering by type: All, My (user preferences), Today, Week.",
			Tags = new[] { "Workshops" }
		)]
		[SwaggerResponse(StatusCodes.Status200OK, "Discover workshops retrieved successfully", typeof(List<DiscoverWorkshopDto>))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid limit parameter")]
		public async Task<IActionResult> GetDiscoverWorkshops([FromQuery] int limit = 8, [FromQuery] DiscoverFilterType filterType = DiscoverFilterType.All, [FromQuery] string? userId = null)
		{
			if (limit <= 0 || limit > 20)
				return BadRequest("Limit must be between 1 and 20");

			var query = new GetDiscoverWorkshopsQuery(limit, filterType, userId);
			var result = await _mediator.Send(query);

			return Ok(result);
		}

		[HttpGet("sessions/calendar")]
		[SwaggerOperation(
			Summary = "Get workshop sessions for calendar view",
			Description = "Retrieves workshop sessions within a date range for calendar display. Optimized for performance by only returning sessions in the specified date range.",
			Tags = new[] { "Workshops" }
		)]
		[ProducesResponseType(typeof(List<WorkshopSessionCalendarDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetSessionsForCalendar(
			[FromQuery] Guid organizationId,
			[FromQuery] DateTime startDate,
			[FromQuery] DateTime endDate)
		{
			if (organizationId == Guid.Empty)
				return BadRequest("OrganizationId cannot be empty.");

			if (startDate > endDate)
				return BadRequest("StartDate must be before EndDate.");

			var query = new GetWorkshopSessionsByDateRangeQuery
			{
				OrganizationId = organizationId,
				StartDate = startDate,
				EndDate = endDate
			};

			var result = await _mediator.Send(query);

			return Ok(result);
		}

		[HttpGet("organization/filter")]
		[SwaggerOperation(
			Summary = "Get workshops by organization with filters",
			Description = "Retrieves a paginated list of workshops for a specific organization with optional filters such as title and status.",
			Tags = new[] { "Workshops" }
		)]
		[ProducesResponseType(typeof(PaginatedList<WorkshopDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetByOrganizationWithFilter(
			[FromQuery] Guid organizationId,
			[FromQuery] string? title,
			[FromQuery] WorkshopStatus? status,
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10)
			{
			if (organizationId == Guid.Empty)
				return BadRequest("OrganizationId cannot be empty.");

			var query = new GetWorkshopsByOrganizationFilterQuery(
				organizationId,
				title,
				status,
				pageNumber,
				pageSize
			);

			var result = await _mediator.Send(query);

			return Ok(result);
		}

		[HttpGet("organization/{organizationId:guid}")]
		[SwaggerOperation(
			Summary = "Get workshops by organization",
			Description = "Retrieves a paginated list of workshops belonging to a specific organization.",
			Tags = new[] { "Workshops" }
		)]
		[ProducesResponseType(typeof(PaginatedList<WorkshopDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetByOrganizationId(
		[FromRoute] Guid organizationId,
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 10)
		{
			var query = new GetWorkshopsByOrganizationIdQuery(organizationId, pageNumber, pageSize);

			var result = await _mediator.Send(query); 

			return Ok(result);
		}

		/// <summary>
		/// Get workshops by organization with time-based status filter
		/// </summary>
		[HttpGet("organization/{organizationId:guid}/time-status")]
		[SwaggerOperation(
			Summary = "Get workshops by organization with time-based status",
			Description = "Retrieves a paginated list of workshops for a specific organization filtered by time-based status (upcoming, ongoing, completed) based on sessions.",
			Tags = new[] { "Workshops" }
		)]
		[ProducesResponseType(typeof(PaginatedList<WorkshopDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetByOrganizationAndTimeStatus(
			[FromRoute] Guid organizationId,
			[FromQuery] int timeStatus = 0, // 0: all, 1: upcoming, 2: ongoing, 3: completed
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10)
		{
			if (organizationId == Guid.Empty)
				return BadRequest("OrganizationId cannot be empty.");

			if (timeStatus < 0 || timeStatus > 3)
				return BadRequest("TimeStatus must be between 0 and 3 (0: all, 1: upcoming, 2: ongoing, 3: completed).");

			var query = new GetWorkshopsByOrganizationAndTimeStatusQuery(
				organizationId,
				timeStatus,
				pageNumber,
				pageSize
			);

			var result = await _mediator.Send(query);

			return Ok(result);
		}

		[HttpGet("search")]
		[SwaggerOperation(
			Summary = "Search workshops",
			Description = "Search workshops by filters and return paged results.",
			Tags = new[] { "Workshops" }
		)]
		[SwaggerResponse(StatusCodes.Status200OK, "Search results", typeof(PaginatedList<SearchWorkshopDto>))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid query")]
		public async Task<ActionResult<PaginatedList<SearchWorkshopDto>>> Search([FromQuery] SearchWorkshopQuery request)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await _mediator.Send(request);
			return Ok(result);
		}

		/// <summary>
		/// Get all workshops (with optional paging).
		/// </summary>
		[HttpGet]
		[SwaggerOperation(
			Summary = "Get all workshops",
			Description = "Get all workshops. Supports paging via query parameters.",
			Tags = new[] { "Workshops" }
		)]
		[SwaggerResponse(StatusCodes.Status200OK, "List of workshops", typeof(PaginatedList<SearchWorkshopDto>))]
		public async Task<ActionResult<PaginatedList<SearchWorkshopDto>>> GetAll([FromQuery] GetAllWorkshopQuery request)
		{
			var result = await _mediator.Send(request);
			return Ok(result);
		}

		/// <summary>
		/// Get a workshop by id.
		/// </summary>
		[HttpGet("{id:guid}")]
		[SwaggerOperation(
			Summary = "Get workshop by id",
			Description = "Retrieve a single workshop by its GUID.",
			Tags = new[] { "Workshops" }
			)]
		[SwaggerResponse(StatusCodes.Status200OK, "Workshop found", typeof(WorkshopDto))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Workshop not found")]
		public async Task<ActionResult<WorkshopDto>> GetById(Guid id)
		{
			var workshop = await _mediator.Send(new GetWorkshopByIdQuery(id)); // adapt to your query/handler
			if (workshop == null)
				return NotFound();
			return Ok(workshop);
		}

		[HttpPost("draft")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Create workshop draft",
			Description = "Creates a draft version of a workshop with basic information such as title, category, organization, and tags. The draft can later be updated with full details before publishing.",
			Tags = new[] { "Workshops" })]
		[SwaggerResponse(StatusCodes.Status201Created, "Workshop draft created successfully", typeof(CreatedDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid or missing request body")]
		[SwaggerResponse(StatusCodes.Status500InternalServerError, "Error while creating workshop draft")]
		public async Task<IActionResult> CreateDraft([FromBody] CreateWorkshopDraftCommand command)
		{
			if (command == null)
				return BadRequest("Request body cannot be null");
			try
			{
				var result = await _mediator.Send(command);
				if (!result.Succeeded || result.Value == null)
					return BadRequest(result.Errors);

				var id = result.Value.Id; 
				return CreatedAtAction(nameof(GetById), new { id }, new { id }); 
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch
			{
				return StatusCode(500, new { message = "An error occurred while creating workshop draft" });
			}
		}

		[HttpPost("draft/details")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Add or update draft details",
			Description = "Add additional information (description, refund policy, etc.) to an existing workshop draft.",
			Tags = new[] { "Workshops" }
		)]
		[SwaggerResponse(StatusCodes.Status201Created, "Workshop created", typeof(CreatedDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
		public async Task<ActionResult> Create([FromBody] CreateWorkshopCommand command)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var id = await _mediator.Send(command);
			return CreatedAtAction(nameof(GetById), new { id = id }, new { id });
		} 

	}
}
