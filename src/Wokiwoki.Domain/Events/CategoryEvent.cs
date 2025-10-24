using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Domain.Events
{
	public class CategoryCreateEvent
	{
		public CategoryCreateEvent(Category category)
		{
			Category = category;
		}

		public Category Category { get; }
	}

	public class CategoryUpdatedEvent
	{
		public CategoryUpdatedEvent(Category category)
		{
			Category = category;
		}

		public Category Category { get; }
	}

	public class CategoryDeletedEvent
	{
		public CategoryDeletedEvent(Category category)
		{
			Category = category;
		}

		public Category Category { get; }
	}
}
