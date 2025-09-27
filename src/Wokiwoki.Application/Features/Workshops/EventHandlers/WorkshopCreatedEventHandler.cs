using MediatR;
using Microsoft.Extensions.Logging; 
using Wokiwoki.Domain.Events;

namespace Wokiwoki.Application.Features.Workshops.EventHandlers
{
	public class WorkshopCreatedEventHandler : INotificationHandler<WorkshopCreatedEvent>
	{
		private readonly ILogger<WorkshopCreatedEventHandler> _logger;

		public WorkshopCreatedEventHandler(ILogger<WorkshopCreatedEventHandler> logger)
		{
			_logger = logger;
		}

		public Task Handle(WorkshopCreatedEvent notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Workshop Domain Event: {DomainEvent}", notification.GetType().Name);

			return Task.CompletedTask;
		}
	}
}
