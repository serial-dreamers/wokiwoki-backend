using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using FluentEmail.MailKitSmtp; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis; 
using System.Text;
using Wokiwoki.Application.Common.Interfaces.Messaging;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Infrastructure.Data.Configurations;
using Wokiwoki.Infrastructure.Data.Messaging;
using Wokiwoki.Infrastructure.Repositories;
using Wokiwoki.Infrastructure.Services;

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    { 
        // ko co la toan
        builder.Services.AddSingleton(TimeProvider.System);

		builder.Services.AddHttpClient();

		builder.Services.AddDbContext<WokiwokiDbContext>(options =>
        {
            options.UseNpgsql(
				builder.Configuration["ConnectionStrings:DefaultConnection"]
            )
       .EnableSensitiveDataLogging()
       .EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
            options.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information);
        });



        builder.Services.AddDataProtection();
        builder.Services.AddIdentityCore<ApplicationUser>(options =>
        {
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<WokiwokiDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

		builder.Services.AddSingleton(x =>
		{
			var config = builder.Configuration.GetSection("AzureBlobStorage").Get<AzureBlobStorageOptions>();
			return new BlobServiceClient(builder.Configuration["AzureBlobStorage:ConnectionString"]);
		});


		builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {

            var redisEndpoint = builder.Configuration["Redis:Endpoint"];
            var redisPassword = builder.Configuration["Redis:Password"];

            // Validate config
            if (string.IsNullOrEmpty(redisEndpoint))
            {
                throw new InvalidOperationException("Redis:Endpoint is not configured in appsettings.json");
            }

            if (string.IsNullOrEmpty(redisPassword))
            {
                throw new InvalidOperationException("Redis:Password is not configured in appsettings.json");
            }

            var options = new ConfigurationOptions
            {
                EndPoints = { { redisEndpoint, 10096 } },
                User = "default",
                Password = redisPassword,
                AbortOnConnectFail = false
            };

            return ConnectionMultiplexer.Connect(options);
        });

        builder.Services.AddSingleton<ServiceBusClient>(sp =>
        {
            var connectionStringServiceBus = builder.Configuration["AzureServiceBus:ConnectionString"];
            return new ServiceBusClient(connectionStringServiceBus);
        });

        builder.Services.AddFluentEmail(builder.Configuration["Smtp:From"])
            .AddRazorRenderer()
            .AddMailKitSender(new SmtpClientOptions
            {
                Server = builder.Configuration["Smtp:SmtpServer"],
                Port = int.Parse(builder.Configuration["Smtp:Port"]!),
                User = builder.Configuration["Smtp:UserName"],
                Password = builder.Configuration["Smtp:Password"],
                UseSsl = false,
                RequiresAuthentication = true,
                SocketOptions = builder.Configuration["Smtp:SecureSocketOptions"] switch
                {
                    "StartTls" => MailKit.Security.SecureSocketOptions.StartTls,
                    "SslOnConnect" => MailKit.Security.SecureSocketOptions.SslOnConnect,
                    _ => MailKit.Security.SecureSocketOptions.Auto
                }
            });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true;
            options.UseSecurityTokenValidators = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                ),

                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,

                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],

                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine("Authentication Failed: " + context.Exception.Message);
                    return Task.CompletedTask;
                }
            };
        });

		builder.Services.AddHttpContextAccessor();


		// Repositories
		builder.Services.AddScoped<IWorkshopRepository, WorkshopRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<ITagRepository, TagRepository>();
        builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
		builder.Services.AddScoped<ITagRepository, TagRepository>(); 
		builder.Services.AddScoped<IUserWorkshopLikeRepository, UserWorkshopLikeRepository>();
		builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
		builder.Services.AddScoped<IWorkshopMediaRepository, WorkshopMediaRepository>();
         

		// Services
		builder.Services.AddHostedService<EmailConsumerHosted>();

        builder.Services.AddSingleton<IMessagePublisher, AzureServiceBusPublisher>();
        builder.Services.AddSingleton<IMessageSubscriber, AzureServiceBusSubscriber>();
        builder.Services.AddTransient<IUuidService, UuidService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddTransient<IIdentityService, IdentityService>();

        builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

        builder.Services.AddTransient<IRefreshTokenService, RefreshTokenService>();
        builder.Services.AddTransient<ITokenService, TokenService>();

		builder.Services.AddScoped<IGoogleService, GoogleService>();

		builder.Services.AddScoped<IUserContext, UserContext>();
		builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

	}
}
