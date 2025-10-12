using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Domain.Events
{
	public class CategoryUpdatedEvent
	{
		public CategoryUpdatedEvent(Category category)
		{
			Category = category;
		}

		public Category Category { get; }
	}
}
