using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Admin.Commands.ApproveOrganization;
using Wokiwoki.Application.Features.Admin.Commands.ApproveWorkshop;
using Wokiwoki.Application.Features.Admin.Commands.RejectOrganization;
using Wokiwoki.Application.Features.Admin.Commands.RejectWorkshop;
using Wokiwoki.Application.Features.Admin.Queries.GetAdminDashboard;
using Wokiwoki.Application.Features.Admin.Queries.GetAdminOrganizations;
using Wokiwoki.Application.Features.Admin.Queries.GetAdminUsers;
using Wokiwoki.Application.Features.Admin.Queries.GetAdminWorkshops;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AdminController : ControllerBase
	{
		private readonly IMediator _mediator;

		public AdminController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("test-admin")]
		[Authorize(Roles = "admin")]
		public IActionResult Test()
		{
			var claims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
			return Ok(claims);  
		}

		#region Dashboard

		/// <summary>
		/// Get admin dashboard statistics
		/// </summary> 
		//[Authorize(Roles = "admin")]
		[HttpGet("dashboard")]
		[SwaggerOperation(
			Summary = "Get admin dashboard",
			Description = "Retrieves comprehensive dashboard statistics for admin including platform revenue (5% fee), total users, organizations, workshops, and charts.",
			Tags = new[] { "Admin" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Dashboard data retrieved successfully", typeof(AdminDashboardDto))]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated")]
		[SwaggerResponse(StatusCodes.Status403Forbidden, "User not authorized (Admin role required)")]
		public async Task<IActionResult> GetDashboard(
			[FromQuery] DateTime? startDate = null,
			[FromQuery] DateTime? endDate = null,
			[FromQuery] string chartGroupBy = "day")
		{
			try
			{
				if (!new[] { "day", "week", "month" }.Contains(chartGroupBy.ToLower()))
					return BadRequest("chartGroupBy must be 'day', 'week', or 'month'");

				var query = new GetAdminDashboardQuery(startDate, endDate, chartGroupBy);
				var result = await _mediator.Send(query);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		#endregion

		#region Users

		/// <summary>
		/// Get all users with filters
		/// </summary> 
		[HttpGet("users")]
		[SwaggerOperation(
			Summary = "Get all users",
			Description = "Retrieves paginated list of users with optional filters by role and search term.",
			Tags = new[] { "Admin", "Users" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Users retrieved successfully", typeof(PaginatedList<AdminUserDto>))]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated")]
		[SwaggerResponse(StatusCodes.Status403Forbidden, "User not authorized")]
		public async Task<IActionResult> GetUsers(
			[FromQuery] string? role = null,
			[FromQuery] string? searchTerm = null,
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 20)
		{
			try
			{
				var query = new GetAdminUsersQuery(role, searchTerm, pageNumber, pageSize);
				var result = await _mediator.Send(query);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		#endregion

		#region Organizations

		/// <summary>
		/// Get all organizations with filters
		/// </summary> 
		[HttpGet("organizations")]
		[SwaggerOperation(
			Summary = "Get all organizations",
			Description = "Retrieves paginated list of organizations with optional filters by status and search term.",
			Tags = new[] { "Admin", "Organizations" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Organizations retrieved successfully", typeof(PaginatedList<AdminOrganizationDto>))]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated")]
		[SwaggerResponse(StatusCodes.Status403Forbidden, "User not authorized")]
		public async Task<IActionResult> GetOrganizations(
			[FromQuery] int? status = null,
			[FromQuery] string? searchTerm = null,
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 20)
		{
			try
			{
				var query = new GetAdminOrganizationsQuery(status, searchTerm, pageNumber, pageSize);
				var result = await _mediator.Send(query);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		/// <summary>
		/// Approve an organization
		/// </summary>
		[HttpPost("organizations/{id:guid}/approve")]
		[SwaggerOperation(
			Summary = "Approve organization",
			Description = "Approves a pending organization, allowing them to create workshops.",
			Tags = new[] { "Admin", "Organizations" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Organization approved successfully")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Organization not found")]
		public async Task<IActionResult> ApproveOrganization(Guid id)
		{
			try
			{
				var command = new ApproveOrganizationCommand(id);
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
		/// Reject/Suspend an organization
		/// </summary>
		[HttpPost("organizations/{id:guid}/reject")]
		[SwaggerOperation(
			Summary = "Reject/Suspend organization",
			Description = "Rejects or suspends an organization with a reason.",
			Tags = new[] { "Admin", "Organizations" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Organization rejected successfully")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request or missing reason")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Organization not found")]
		public async Task<IActionResult> RejectOrganization(Guid id, [FromBody] RejectRequest request)
		{
			try
			{
				var command = new RejectOrganizationCommand(id, request.Reason);
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

		#endregion

		#region Workshops

		/// <summary>
		/// Get all workshops with filters
		/// </summary> 
		[HttpGet("workshops")]
		[SwaggerOperation(
			Summary = "Get all workshops",
			Description = "Retrieves paginated list of workshops with optional filters by status, organization, and search term.",
			Tags = new[] { "Admin", "Workshops" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Workshops retrieved successfully", typeof(PaginatedList<AdminWorkshopDto>))]
		[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated")]
		[SwaggerResponse(StatusCodes.Status403Forbidden, "User not authorized")]
		public async Task<IActionResult> GetWorkshops(
			[FromQuery] int? status = null,
			[FromQuery] string? searchTerm = null,
			[FromQuery] Guid? organizationId = null,
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10)
		{
			try
			{
				var query = new GetAdminWorkshopsQuery(status, searchTerm, organizationId, pageNumber, pageSize);
				var result = await _mediator.Send(query);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		/// <summary>
		/// Approve a workshop
		/// </summary>
		[HttpPost("workshops/{id:guid}/approve")]
		[SwaggerOperation(
			Summary = "Approve workshop",
			Description = "Approves a pending workshop, making it published and visible to users.",
			Tags = new[] { "Admin", "Workshops" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Workshop approved successfully")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Workshop not found")]
		public async Task<IActionResult> ApproveWorkshop(Guid id)
		{
			try
			{
				var command = new ApproveWorkshopCommand(id);
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
		/// Reject/Hide a workshop
		/// </summary>
		[HttpPost("workshops/{id:guid}/reject")]
		[SwaggerOperation(
			Summary = "Reject/Hide workshop",
			Description = "Rejects or hides a workshop with a reason.",
			Tags = new[] { "Admin", "Workshops" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Workshop rejected successfully")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request or missing reason")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Workshop not found")]
		public async Task<IActionResult> RejectWorkshop(Guid id, [FromBody] RejectRequest request)
		{
			try
			{
				var command = new RejectWorkshopCommand(id, request.Reason);
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

		#endregion
	}

	/// <summary>
	/// Request model for rejection with reason
	/// </summary>
	public class RejectRequest
	{
		public string Reason { get; set; } = null!;
	}
}

