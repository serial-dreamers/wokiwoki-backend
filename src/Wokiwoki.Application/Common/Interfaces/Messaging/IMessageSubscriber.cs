namespace Wokiwoki.Application.Common.Interfaces.Messaging
{
	public interface IMessageSubscriber
	{
		Task SubscribeAsync<T>(string queueName, Func<T, Task> handler, CancellationToken ct = default);
		ValueTask DisposeAsync();
	}
}
