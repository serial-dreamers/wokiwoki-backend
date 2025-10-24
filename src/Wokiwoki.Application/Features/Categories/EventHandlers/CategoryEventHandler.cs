using Microsoft.Extensions.Logging;
using Wokiwoki.Domain.Events;

namespace Wokiwoki.Application.Features.Categories.EventHandlers
{
	public class CategoryEventHandler
	{
		private readonly ILogger<CategoryEventHandler> _logger;

		public CategoryEventHandler(ILogger<CategoryEventHandler> logger)
		{
			_logger = logger;
		}

		public Task Handle(CategoryCreateEvent notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Category Domain Event: {DomainEvent}", notification.GetType().Name);
			return Task.CompletedTask;
		}

		public Task Handle(CategoryUpdatedEvent notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Category updated: {Name}", notification.Category.Name);
			return Task.CompletedTask;
		}

		public Task Handle(CategoryDeletedEvent notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Category deleted: {Name}", notification.Category.Name);
			return Task.CompletedTask;
		}
	}
}
