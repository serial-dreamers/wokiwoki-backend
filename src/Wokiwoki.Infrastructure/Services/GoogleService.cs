using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs;

namespace Wokiwoki.Infrastructure.Services
{
	public class GoogleService : IGoogleService
	{ 
		private readonly IConfiguration _configuration;
		private readonly ILogger<GoogleService> _logger;

		public GoogleService( 
		IConfiguration configuration,
		ILogger<GoogleService> logger)
		{ 
			_configuration = configuration;
			_logger = logger;
		}

		public async Task<GoogleUserInfoDto?> GetUserByTokenId(string tokenId)
		{
			var clientId = _configuration["Google:ClientId"];

			var payload = await GoogleJsonWebSignature.ValidateAsync(
				tokenId,
				new GoogleJsonWebSignature.ValidationSettings
				{
					Audience = new[] { clientId }
				});

			if (payload != null){
				return new GoogleUserInfoDto
				{
					FullName = payload.Name,
					Email = payload.Email,
					Picture = payload.Picture,
					ProviderKey = payload.Subject
				};
			} 
			return null;
		}

	}
}
