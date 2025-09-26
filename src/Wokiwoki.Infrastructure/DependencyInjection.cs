using dotenv.net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Infrastructure.Data;
using Wokiwoki.Infrastructure.Repositories;
using Wokiwoki.Infrastructure.Services;

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
	public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
	{ 
		var config = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: true)
			.AddEnvironmentVariables()
			.Build();

		// ko co la toan
		builder.Services.AddSingleton(TimeProvider.System);

		builder.Services.AddDbContext<WokiwokiDbContext>(options =>
			options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

		builder.Services.AddDataProtection();
		builder.Services.AddIdentityCore<ApplicationUser>(options =>
		{

		})
		.AddRoles<IdentityRole>()
		.AddEntityFrameworkStores<WokiwokiDbContext>()
		.AddSignInManager()
		.AddDefaultTokenProviders();


		// Repositories
		builder.Services.AddScoped<IWorkshopRepository, WorkshopRepository>();

		// Services
		builder.Services.AddScoped<IGuidGenerator, Uuid7GuidGenerator>();
	}
}
