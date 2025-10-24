using System.Security.Claims; 

namespace Wokiwoki.Infrastructure.Data.Extensions
{
	internal static class ClaimsPrincipalExtensions
	{
		public static string GetUserId(this ClaimsPrincipal? principal)
		{
			var userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				throw new ApplicationException("User id is unavailable");
			return userId;
		}


	}
}
