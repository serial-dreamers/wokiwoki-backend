using Microsoft.Extensions.Logging;
using Wokiwoki.Application.Features.Categories.EventHandlers;
using Wokiwoki.Domain.Events;

namespace Wokiwoki.Application.Features.Tags.EventHandlers
{
	public class TagEventHandler
	{
		private readonly ILogger<TagEventHandler> _logger;

		public TagEventHandler(ILogger<TagEventHandler> logger)
		{
			_logger = logger;
		}

		public Task Handle(TagCreatedEvent notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Tag Domain Event: {DomainEvent}", notification.GetType().Name);
			return Task.CompletedTask;
		}

		public Task Handle(TagUpdatedEvent notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Tag updated: {Name}", notification.Tag.Name);
			return Task.CompletedTask;
		}

		public Task Handle(TagDeletedEvent notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Tag deleted: {Name}", notification.Tag.Name);
			return Task.CompletedTask;
		}
	}
}
