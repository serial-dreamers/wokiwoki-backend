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
using Wokiwoki.Application.Features.Organizations.Queries.GetOrganizationById;
using Wokiwoki.Application.Features.Organizations.Queries.GetOrganizationsByCategory;
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
		/// Create a new organization.
		/// </summary>
		/// <remarks>
		/// Accepts multipart/form-data including files and fields to create a new organization.
		/// Returns the created resource ID and sets the Location header pointing to the created organization.
		/// </remarks>
		/// <param name="request">The organization creation request including name, contact info, address, and optional logo file.</param>
		/// <returns>Returns 201 Created with the organization ID and Location header.</returns>
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

	}
}
