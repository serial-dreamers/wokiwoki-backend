using dotenv.net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting; 
using System.Reflection; 

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
	public static void AddApplicationServices(this IHostApplicationBuilder builder)
	{
		DotEnv.Load();

		builder.Configuration
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.AddEnvironmentVariables();

		//builder.Services.AddSingleton<TimeProvider>(TimeProvider.System); 

		builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

		builder.Services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
		});
	}

}

