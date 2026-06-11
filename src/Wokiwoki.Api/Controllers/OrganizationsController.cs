using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Api.Request;
using Wokiwoki.Application.Common.Interfaces.Services; 
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Organizations.Commands.CreateOrganization;
using Wokiwoki.Application.Features.Organizations.Commands.FollowOrganization;
using Wokiwoki.Application.Features.Organizations.Commands.UnfollowOrganization;
using Wokiwoki.Application.Features.Organizations.Commands.UpdateOrganizationInfo;
using Wokiwoki.Application.Features.Organizations.Commands.UpdateOrganizationLogo;
using Wokiwoki.Application.Features.Organizations.Queries.GetOrganizationById;
using Wokiwoki.Application.Features.Organizations.Queries.GetOrganizationsByCategory;
using Wokiwoki.Application.Features.Organizations.Queries.GetTopOrganizationsByFollowerCount;
using Wokiwoki.Application.Features.Organizations.Queries.GetMyOrganization;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrganizationsController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly IBlobStorageService _blobStorageService;
		public OrganizationsController(IMediator mediator, IBlobStorageService blobStorageService)
		{
			_mediator = mediator;
			_blobStorageService = blobStorageService;
		}

		/// <summary>
		/// Get top organizations by follower count
		/// </summary>
		[HttpGet("top-by-followers")]
		[SwaggerOperation(
			Summary = "Get top organizations by follower count",
			Description = "Retrieve the top organizations sorted by follower count.",
			Tags = new[] { "Organizations" })]
		[ProducesResponseType(typeof(List<OrganizationDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetTopOrganizationsByFollowerCount([FromQuery] int limit = 6)
		{
			if (limit <= 0 || limit > 100)
				return BadRequest(new { message = "Limit must be between 1 and 100." });

			var query = new GetTopOrganizationsByFollowerCountQuery(limit);
			var result = await _mediator.Send(query);

			return Ok(result);
		}
		 
		[Authorize]
		[HttpPost]
		[Consumes("multipart/form-data")]
		[SwaggerOperation(
			Summary = "Create a new organization",
			Description = "Upload optional logo and create a new organization with name, contact info, and address."
		)]
		[ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> Create([FromForm] CreateOrganizationRequest request)
		{
			string? logoUrl = string.Empty;

			if (request.LogoFile != null)
			{
				using var stream = request.LogoFile.OpenReadStream();
				logoUrl = await _blobStorageService.UploadFileAsync(
					stream,
					request.LogoFile.FileName,
					request.LogoFile.ContentType,
					request.LogoFile.Length,
					BlobContainerType.OrganizationLogos
				);
			}

			var command = new CreateOrganizationCommand(
				request.Name,
				request.Description,
				request.ContactEmail,
				request.ContactPhone,
				request.Street,
				request.Commune,
				request.Province,
				LogoUrl: logoUrl 
			);

			var id = await _mediator.Send(command);
			// Return 201 Created with Location header referencing GetById
			return CreatedAtAction(nameof(GetById), new { id = id }, new { id });
		}


		/// <summary>
		/// Retrieve an organization by its ID.
		/// </summary>
		/// <param name="id">The unique identifier of the organization.</param>
		/// <returns>Returns the details of the organization if found, otherwise 404 Not Found.</returns>
		[HttpGet("{id:guid}")]
		[SwaggerOperation(
			Summary = "Get an organization by ID",
			Description = "Retrieve the full details of a specific organization using its unique identifier."
		)]
		[ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetById(Guid id)
		{
			var organization = await _mediator.Send(new GetOrganizationByIdQuery(id)); // adapt to your query/handler
			if (organization == null)
				return NotFound();
			return Ok(organization);
		}

		/// <summary>
		/// Get organizations by categories
		/// </summary>
		[HttpPost("by-categories")]
		[Consumes("application/json")]
		[SwaggerOperation(
			Summary = "Get organizations by categories",
			Description = "Retrieve organizations grouped by categories that have workshops in the specified categories.",
			Tags = new[] { "Organizations" })]
		[ProducesResponseType(typeof(List<OrganizationsByCategoryDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetByCategories([FromBody] GetOrganizationsByCategoryRequest request)
		{
			if (request.CategoryIds == null || !request.CategoryIds.Any())
				return BadRequest(new { message = "CategoryIds are required." });

			var query = new GetOrganizationsByCategoryQuery(request.CategoryIds, request.LimitPerCategory ?? 3);
			var result = await _mediator.Send(query);
			
			return Ok(result);
		}

		/// <summary>
		/// Follow an organization
		/// </summary>
		[Authorize]
		[HttpPost("{id:guid}/follow")]
		[SwaggerOperation(
			Summary = "Follow organization",
			Description = "Follow an organization to get notified about their workshops.",
			Tags = new[] { "Organizations" })]
		[ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> FollowOrganization(Guid id)
		{
			var command = new FollowOrganizationCommand(id);
			var result = await _mediator.Send(command);

			if (!result.Succeeded)
				return BadRequest(result);

			return Ok(result);
		}

		/// <summary>
		/// Unfollow an organization
		/// </summary>
		[Authorize]
		[HttpDelete("{id:guid}/follow")]
		[SwaggerOperation(
			Summary = "Unfollow organization",
			Description = "Stop following an organization.",
			Tags = new[] { "Organizations" })]
		[ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> UnfollowOrganization(Guid id)
		{
			var command = new UnfollowOrganizationCommand(id);
			var result = await _mediator.Send(command);

			if (!result.Succeeded)
				return BadRequest(result);

			return Ok(result);
		}

		/// <summary>
		/// Get my organization
		/// </summary>
		[Authorize]
		[HttpGet("my-organization")]
		[SwaggerOperation(
			Summary = "Get my organization",
			Description = "Get the organization owned by the authenticated user.",
			Tags = new[] { "Organizations" })]
		[ProducesResponseType(typeof(OrganizationDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetMyOrganization()
		{
			try
			{
				var result = await _mediator.Send(new GetMyOrganizationQuery());
				if (result == null)
					return NotFound(new { message = "Organization not found" });
				return Ok(result);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(ex.Message);
			}
		}

		/// <summary>
		/// Update organization info
		/// </summary>
		[Authorize]
		[HttpPut("info")]
		[SwaggerOperation(
			Summary = "Update organization info",
			Description = "Update organization information (name, description, contact, address).",
			Tags = new[] { "Organizations" })]
		[ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> UpdateOrganizationInfo([FromBody] UpdateOrganizationInfoCommand command)
		{
			try
			{
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
		/// Update organization logo
		/// </summary>
		[Authorize]
		[HttpPut("logo")]
		[Consumes("multipart/form-data")]
		[SwaggerOperation(
			Summary = "Update organization logo",
			Description = "Upload and update organization logo.",
			Tags = new[] { "Organizations" })]
		[ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> UpdateOrganizationLogo([FromForm] UpdateOrganizationLogoRequest request)
		{
			var logoFile = request.LogoFile;
			if (logoFile == null || logoFile.Length == 0)
				return BadRequest(new { message = "Logo file is required" });

			using var stream = logoFile.OpenReadStream();
			var logoUrl = await _blobStorageService.UploadFileAsync(
				stream,
				logoFile.FileName,
				logoFile.ContentType,
				logoFile.Length,
				BlobContainerType.OrganizationLogos
			);

			var command = new UpdateOrganizationLogoCommand(logoUrl);
			var result = await _mediator.Send(command);

			if (!result.Succeeded)
				return BadRequest(result);

			return Ok(new { logoUrl, result });
		}
	}
}
