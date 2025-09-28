namespace Wokiwoki.Application.Common.Interfaces.Messaging
{
	public interface IMessagePublisher
	{
		Task PublishAsync<T>(T message, string queueName, CancellationToken ct = default);
	}
}
