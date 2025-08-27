using dotenv.net;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
	public static void AddWebAPIServices(this IHostApplicationBuilder builder)
	{

		builder.Configuration.AddEnvironmentVariables();

		builder.Services.AddEndpointsApiExplorer();

		builder.Services.AddControllers().AddJsonOptions(options =>
			options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

		builder.Services.AddSwaggerGen();

	}
}

