using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Domain.Events
{
	public class WorkshopCreatedEvent : BaseEvent
	{
		public WorkshopCreatedEvent(Workshop workshop)
		{
			Workshop = workshop;
		}

		public Workshop Workshop { get; }
	}
}
