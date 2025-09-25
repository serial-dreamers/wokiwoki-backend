namespace Wokiwoki.Domain.Common
{
	public abstract class BaseEvent 
	{
		public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
	}
}
