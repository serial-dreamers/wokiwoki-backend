using dotenv.net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Wokiwoki.Api.Middlewares;
using Wokiwoki.Infrastructure.Data;
using Wokiwoki.Infrastructure.Identity;

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
	public static void AddWebAPIServices(this IHostApplicationBuilder builder)
	{  

		builder.Configuration.AddEnvironmentVariables();

		builder.Services.AddEndpointsApiExplorer();

		builder.Services.AddControllers().AddJsonOptions(options =>
		{
			options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
			options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
		});

		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "Wokiwoki_api", Version = "v1" });

			// Configure Swagger for JWT Authentication
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				In = ParameterLocation.Header,
				Description = "Please enter token",
				Name = "Authorization",
				Type = SecuritySchemeType.Http,
				BearerFormat = "JWT",
				Scheme = "bearer"
			});
			c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						new string[] {}
					}
				});

			c.EnableAnnotations();
		});

		builder.Services.AddCors(options =>
		{
			options.AddPolicy("AllowFrontend",
				builder =>
				{
					builder.WithOrigins(
						"http://localhost:5173",
						"https://www.wokiwoki.com/"
						)
						   .AllowAnyMethod()
						   .AllowAnyHeader()
						   .AllowCredentials();
				});
		});

		// Midlewares
		builder.Services.AddSingleton<Stopwatch>();
		builder.Services.AddScoped<PerformanceMiddleware>();
		builder.Services.AddScoped<GlobalExceptionMiddleware>();


	}
}

