using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wokiwoki.Api.Request;
using Wokiwoki.Application.Common.Interfaces.Services; 
using Wokiwoki.Application.Features.Organizations.Commands.CreateOrganization;
using Wokiwoki.Application.Features.Organizations.Queries.GetOrganizationById;
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
		/// Accepts multipart/form-data (files and fields) and creates a new organization.
		/// Returns the created resource id and Location header.
		/// </remarks>
		[HttpPost]
		[Consumes("multipart/form-data")]
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
		/// Get an organization by id.
		/// </summary>
		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var organization = await _mediator.Send(new GetOrganizationByIdQuery(id)); // adapt to your query/handler
			if (organization == null)
				return NotFound();
			return Ok(organization);
		} 

	}
}
