using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
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
		var config = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: true)
			.AddEnvironmentVariables()
			.Build();

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
				Encoding.UTF8.GetBytes(config["Jwt:Key"]!)
			),

				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,

				ValidIssuer = config["Jwt:Issuer"],
				ValidAudience = config["Jwt:Audience"],

				ClockSkew = TimeSpan.Zero // không cho lệch thời gian
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

		// Midlewares
		builder.Services.AddSingleton<Stopwatch>();
		builder.Services.AddScoped<PerformanceMiddleware>();
		builder.Services.AddScoped<GlobalExceptionMiddleware>();


	}
}

