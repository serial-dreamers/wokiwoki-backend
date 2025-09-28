using Azure.Messaging.ServiceBus;
using System.Collections.Concurrent;
using System.Text.Json;
using Wokiwoki.Application.Common.Interfaces.Messaging;

namespace Wokiwoki.Infrastructure.Data.Messaging
{
	public class AzureServiceBusPublisher : IMessagePublisher, IAsyncDisposable
	{
		private readonly ServiceBusClient _client;
		private readonly ConcurrentDictionary<string, ServiceBusSender> _senders = new();

		public AzureServiceBusPublisher(ServiceBusClient client)
		{
			_client = client;
		}

		public async Task PublishAsync<T>(T message, string queueName, CancellationToken ct = default)
		{
			var sender = _senders.GetOrAdd(queueName, q => _client.CreateSender(q));
			var body = JsonSerializer.Serialize(message);
			var msg = new ServiceBusMessage(body);
			await sender.SendMessageAsync(msg, ct);
		}

		public async ValueTask DisposeAsync()
		{
			foreach (var sender in _senders.Values)
				await sender.DisposeAsync();
			await _client.DisposeAsync();
		}
	}
}
