using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wokiwoki.Application.Common.Interfaces.Services;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AddressController : ControllerBase
	{
		private readonly HttpClient _httpClient;
		private readonly IGoongMapService _goongMapService;

		public AddressController(HttpClient httpClient, IGoongMapService goongMapService)
		{
			_httpClient = httpClient;
			_goongMapService = goongMapService;
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

		/// <summary>
		/// Geocode an address to get coordinates (latitude, longitude).
		/// </summary>
		[HttpGet("geocode")]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Geocode address",
			Description = "Converts an address string to geographic coordinates (latitude, longitude).",
			Tags = new[] { "Address" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Coordinates returned")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Address parameter is missing or invalid")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Address not found")]
		public async Task<IActionResult> GeocodeAddress([FromQuery] string address, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(address))
				return BadRequest(new { message = "address parameter is required" });

			try
			{
				var coordinates = await _goongMapService.GetCoordinatesAsync(address);
				
				if (coordinates.HasValue)
					return Ok(new { lat = coordinates.Value.lat, lng = coordinates.Value.lng });
				
				return NotFound(new { message = "Address not found" });
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to geocode address", detail = ex.Message });
			}
		}

		/// <summary>
		/// Get place suggestions (autocomplete) for an input string.
		/// </summary>
		[HttpGet("place-suggestions")]
		[Produces("application/json")]
		[SwaggerOperation(
			Summary = "Get place suggestions",
			Description = "Returns a list of place suggestions based on the input string using Goong Maps Autocomplete API.",
			Tags = new[] { "Address" })]
		[SwaggerResponse(StatusCodes.Status200OK, "Place suggestions returned")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Input parameter is missing")]
		public async Task<IActionResult> GetPlaceSuggestions([FromQuery] string input, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(input))
				return BadRequest(new { message = "input parameter is required" });

			try
			{
				var suggestions = await _goongMapService.GetPlaceSuggestionsAsync(input, cancellationToken);
				return Ok(suggestions);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to get place suggestions", detail = ex.Message });
			}
		}

	}
}
