using dotenv.net;
using Microsoft.Extensions.Hosting; 
using System.Reflection; 

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
	public static void AddApplicationServices(this IHostApplicationBuilder builder)
	{
		DotEnv.Load();

		//builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

		builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

		builder.Services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
		});
	}

}

