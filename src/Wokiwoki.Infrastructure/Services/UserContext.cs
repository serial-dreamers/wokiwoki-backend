using Microsoft.AspNetCore.Http;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Infrastructure.Data.Extensions;

namespace Wokiwoki.Infrastructure.Services
{
	internal sealed class UserContext(IHttpContextAccessor httpContextAccessor)
	: IUserContext
	{
		public string UserId =>
			httpContextAccessor
				.HttpContext?
				.User
				.GetUserId() ??
			throw new ApplicationException("User context is unavailable");

		public bool IsAuthenticated =>
			httpContextAccessor
				.HttpContext?
				.User
				.Identity?
				.IsAuthenticated ??
			throw new ApplicationException("User context is unavailable");
	}
}
