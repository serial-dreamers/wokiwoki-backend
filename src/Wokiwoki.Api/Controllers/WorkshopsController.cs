using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Workshops.Commands.CreateWorkshop;
using Wokiwoki.Application.Features.Workshops.Queries.GetFilterPagedWorkshopsQuery;
using Wokiwoki.Application.Features.Workshops.Queries.GetWorkshop;

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




		/// <summary>
		/// Create a new workshop.
		/// </summary>
		/// <remarks>
		/// Accepts multipart/form-data (files and fields) and creates a new workshop.
		/// Returns the created resource id and Location header.
		/// </remarks>
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


		/// <summary>
		/// Search workshops.
		/// </summary>
		/// <remarks>
		/// Use query parameters to filter and paginate results (e.g. ?q=react&page=1&pageSize=20).
		/// </remarks>
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


		
	}
}
