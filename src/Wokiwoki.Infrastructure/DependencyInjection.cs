using Azure.Messaging.ServiceBus; 
using FluentEmail.MailKitSmtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Wokiwoki.Application.Common.Interfaces.Messaging;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services; 
using Wokiwoki.Infrastructure.Data.Messaging;
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

		builder.Services.AddSingleton<ServiceBusClient>(sp =>
		{
			var  connectionStringServiceBus = config["AzureServiceBus:ConnectionString"];
			return new ServiceBusClient(connectionStringServiceBus);
		});

		builder.Services.AddFluentEmail(config["Smtp:From"])
			.AddRazorRenderer()
			.AddMailKitSender(new SmtpClientOptions
			{
				Server = config["Smtp:SmtpServer"],
				Port = int.Parse(config["Smtp:Port"]),
				User = config["Smtp:UserName"],
				Password = config["Smtp:Password"],
				UseSsl = false,
				RequiresAuthentication = true,
				SocketOptions = config["Smtp:SecureSocketOptions"] switch
				{
					"StartTls" => MailKit.Security.SecureSocketOptions.StartTls,
					"SslOnConnect" => MailKit.Security.SecureSocketOptions.SslOnConnect,
					_ => MailKit.Security.SecureSocketOptions.Auto
				}
			});


		// Repositories
		builder.Services.AddScoped<IWorkshopRepository, WorkshopRepository>(); 
		builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
		builder.Services.AddScoped<ITagRepository, TagRepository>();


		// Services
		builder.Services.AddHostedService<EmailConsumerHosted>();

		builder.Services.AddSingleton<IMessagePublisher, AzureServiceBusPublisher>();
		builder.Services.AddSingleton<IMessageSubscriber, AzureServiceBusSubscriber>(); 
		builder.Services.AddTransient<IUuidService, UuidService>();
		builder.Services.AddScoped<IEmailService, EmailService>();
		builder.Services.AddTransient<IIdentityService, IdentityService>();

	}
}
