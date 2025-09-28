using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Wokiwoki.Application.Common.Interfaces.Messaging;

namespace Wokiwoki.Infrastructure.Data.Messaging
{
	public class AzureServiceBusSubscriber : IMessageSubscriber
	{
		private readonly ServiceBusClient _client;
		private readonly ILogger<AzureServiceBusSubscriber>? _logger;
		private ServiceBusProcessor? _processor;

		public AzureServiceBusSubscriber(ServiceBusClient client, ILogger<AzureServiceBusSubscriber>? logger = null)
		{
			_client = client;
			_logger = logger;
		}

		public async Task SubscribeAsync<T>(string queueName, Func<T, Task> handler, CancellationToken ct = default)
		{
			var options = new ServiceBusProcessorOptions
			{
				MaxConcurrentCalls = 1, 
				AutoCompleteMessages = false,  
				ReceiveMode = ServiceBusReceiveMode.PeekLock  
			};

			_processor = _client.CreateProcessor(queueName, options);

			_processor.ProcessMessageAsync += async args =>
			{
				try
				{
					var body = args.Message.Body.ToString();
					var message = JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});

					if (message != null)
					{
						await handler(message);
					}
					else
					{
						_logger?.LogWarning("Failed to deserialize message: {Body}", body);
					}

					await args.CompleteMessageAsync(args.Message, ct);
				}
				catch (Exception ex)
				{
					_logger?.LogError(ex, "Error processing message"); 
					await args.AbandonMessageAsync(args.Message, null, ct);
				}
			};

			_processor.ProcessErrorAsync += args =>
			{
				_logger?.LogError(args.Exception, "ServiceBusProcessor error: {ErrorSource}", args.ErrorSource);
				return Task.CompletedTask;
			};

			await _processor.StartProcessingAsync(ct);
		}


		public async ValueTask DisposeAsync()
		{
			if (_processor != null)
			{
				await _processor.StopProcessingAsync();
				await _processor.DisposeAsync();
				_processor = null;
			}
		}
	}

}
