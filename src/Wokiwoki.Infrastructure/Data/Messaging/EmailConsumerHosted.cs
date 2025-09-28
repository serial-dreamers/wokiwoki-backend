using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wokiwoki.Application.Common.Interfaces.Messaging;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Infrastructure.Data.Messaging
{
	public class EmailConsumerHosted : BackgroundService
	{
		private readonly IMessageSubscriber _subscriber;
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<EmailConsumerHosted> _logger;

		public EmailConsumerHosted(
			IMessageSubscriber subscriber,
			IServiceScopeFactory scopeFactory,
			ILogger<EmailConsumerHosted> logger)
		{
			_subscriber = subscriber;
			_scopeFactory = scopeFactory;
			_logger = logger;
		}

		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			try
			{
				await _subscriber.DisposeAsync();
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Error while disposing subscriber in StopAsync.");
			}

			await base.StopAsync(cancellationToken);
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{ 
			return _subscriber.SubscribeAsync<EmailVerificationRequest>("email-queue", async msg =>
			{
				using var scope = _scopeFactory.CreateScope();
				try
				{
					var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
					 
					await emailService.SendEmailVerificationCode(msg /*, stoppingToken */);
					_logger.LogInformation("Sent verify email to {To}", msg.To);
				}
				catch (Exception ex)
				{
					 
					_logger.LogError(ex, "Error sending email to {To}", msg.To); 
				}
			}, stoppingToken);  
		}
	}
}
