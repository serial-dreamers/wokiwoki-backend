using Wokiwoki.Api.Middlewares;

namespace Wokiwoki.Api
{
	public static class MiddlewareExtensions
	{
		public static IApplicationBuilder UseWebAPIMiddlewares(this IApplicationBuilder app)
		{ 
			app.UseMiddleware<GlobalExceptionMiddleware>();
			app.UseMiddleware<LoggingMiddleware>();
			app.UseMiddleware<PerformanceMiddleware>();

			return app;
		}
	}
}
