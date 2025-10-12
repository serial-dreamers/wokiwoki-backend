using Microsoft.Extensions.Logging;
using Wokiwoki.Domain.Events;

namespace Wokiwoki.Application.Features.Categories.EventHandlers
{
	public class CategoryCreatedEventHandler
	{
		private readonly ILogger<CategoryCreatedEventHandler> _logger;

		public CategoryCreatedEventHandler(ILogger<CategoryCreatedEventHandler> logger)
		{
			_logger = logger;
		}

		public Task Handle(CategoryCreatedEvent notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Category Domain Event: {DomainEvent}", notification.GetType().Name);
			return Task.CompletedTask;
		}
	}
}
