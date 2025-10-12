using Wokiwoki.Application.DTOs;

namespace Wokiwoki.Application.Common.Interfaces.Services
{
	public interface IGoogleService
	{
		Task<GoogleUserInfoDto?> GetUserByTokenId(string tokenId);
	}
}
