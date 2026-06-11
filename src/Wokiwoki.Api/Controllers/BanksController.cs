using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace Wokiwoki.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BanksController : ControllerBase
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration _configuration;


		public BanksController(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			_configuration = configuration;
		}

		[HttpGet]
		public async Task<IActionResult> GetBankList()
		{
			var url = _configuration["VietQrApi:BankListUrl"];
			if (string.IsNullOrEmpty(url))
				return StatusCode(500, "Bank API URL not configured");

			var response = await _httpClient.GetFromJsonAsync<object>(url);

			return Ok(response);
		}
	}
}
