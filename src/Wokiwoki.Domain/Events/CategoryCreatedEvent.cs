using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Domain.Events
{
	public class CategoryCreatedEvent
	{
		public CategoryCreatedEvent(Category category)
		{
			Category = category;
		}

		public Category Category { get; }
	}
}
