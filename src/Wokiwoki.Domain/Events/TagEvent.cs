using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Domain.Events
{
	public class TagCreatedEvent
	{
		public TagCreatedEvent(Tag tag)
		{
			Tag = tag;
		}

		public Tag Tag { get; }
	}

	public class TagUpdatedEvent
	{
		public TagUpdatedEvent(Tag tag)
		{
			Tag = tag;
		}

		public Tag Tag { get; }
	}

	public class TagDeletedEvent
	{
		public TagDeletedEvent(Tag tag)
		{
			Tag = tag;
		}

		public Tag Tag { get; }
	}
}
