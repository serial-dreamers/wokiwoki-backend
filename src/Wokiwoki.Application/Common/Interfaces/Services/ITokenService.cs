namespace Wokiwoki.Application.Common.Interfaces.Services
{
	public interface ITokenService
	{
		string GenerateToken(string userId, string email, IEnumerable<string> roles);
	}
}
