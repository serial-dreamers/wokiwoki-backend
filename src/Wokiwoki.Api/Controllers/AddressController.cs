using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AddressController : ControllerBase
	{
		private readonly HttpClient _httpClient;

		public AddressController(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}


		/// <summary>
		/// Retrieve list of provinces from external address service.
		/// </summary>
		/// <remarks>
		/// Calls the external Address Kit service and returns the raw JSON payload.
		/// </remarks>
		[HttpGet("provinces")]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Get provinces",
			Description = "Fetches the list of provinces from the external Address Kit service.",
			Tags = new[] { "Address" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Provinces returned (raw JSON)")]
		[SwaggerResponse(StatusCodes.Status502BadGateway, "Upstream service returned an error")]
		public async Task<IActionResult> GetProvinces(CancellationToken cancellationToken)
		{
			var url = "https://production.cas.so/address-kit/2025-07-01/provinces";

			try
			{
				using var response = await _httpClient.GetAsync(url, cancellationToken);
				var content = await response.Content.ReadAsStringAsync(cancellationToken);

				if (response.IsSuccessStatusCode)
					return Content(content, "application/json");

				// Propagate upstream status code and body
				return StatusCode((int)response.StatusCode, content);
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				return StatusCode(StatusCodes.Status499ClientClosedRequest); // client cancelled
			}
			catch (Exception ex)
			{
				// Optionally log `ex`
				return StatusCode(StatusCodes.Status502BadGateway, new { message = "Failed to retrieve provinces", detail = ex.Message });
			}
		}

		/// <summary>
		/// Retrieve communes for a given province code from external address service.
		/// </summary>
		/// <remarks>
		/// Provide the provinceCode as a query parameter. Returns the raw JSON payload from the upstream service.
		/// </remarks>
		[HttpGet("communes")]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Get communes by province code",
			Description = "Fetches communes for the specified provinceCode from the external Address Kit service.",
			Tags = new[] { "Address" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Communes returned (raw JSON)")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "provinceCode is missing or invalid")]
		[SwaggerResponse(StatusCodes.Status502BadGateway, "Upstream service returned an error")]
		public async Task<IActionResult> GetCommunes([FromQuery] string provinceCode, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(provinceCode))
				return BadRequest(new { message = "provinceCode is required" });

			var url = $"https://production.cas.so/address-kit/2025-07-01/provinces/{Uri.EscapeDataString(provinceCode)}/communes";

			try
			{
				using var response = await _httpClient.GetAsync(url, cancellationToken);
				var content = await response.Content.ReadAsStringAsync(cancellationToken);

				if (response.IsSuccessStatusCode)
					return Content(content, "application/json");

				return StatusCode((int)response.StatusCode, content);
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				return StatusCode(StatusCodes.Status499ClientClosedRequest);
			}
			catch (Exception ex)
			{
				// Optionally log `ex`
				return StatusCode(StatusCodes.Status502BadGateway, new { message = "Failed to retrieve communes", detail = ex.Message });
			}
		}

	}
}
