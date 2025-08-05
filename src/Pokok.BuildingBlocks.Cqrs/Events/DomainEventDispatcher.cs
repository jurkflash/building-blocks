using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pokok.BuildingBlocks.Domain.Events;
using System.Diagnostics;

namespace Pokok.BuildingBlocks.Cqrs.Events
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DomainEventDispatcher> _logger;

        public DomainEventDispatcher(IServiceProvider serviceProvider, ILogger<DomainEventDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in domainEvents)
            {
                _logger.LogInformation("Dispatching domain event: {EventType}", domainEvent.GetType().Name);

                var eventType = domainEvent.GetType();
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
                var handlers = _serviceProvider.GetServices(handlerType);

                foreach (var handler in handlers)
                {
                    var handleMethod = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.Handle));
                    if (handleMethod is null)
                    {
                        _logger.LogWarning("Handle method not found for {HandlerType}", handler.GetType().FullName);
                        continue;
                    }

                    try
                    {
                        _logger.LogDebug("Invoking handler {HandlerType} for event {EventType}", handler.GetType().FullName, eventType.Name);

                        var task = (Task?)handleMethod.Invoke(handler, new object[] { domainEvent, cancellationToken });
                        if (task != null)
                            await task.ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception in handler {HandlerType} for domain event {EventType}", handler.GetType().FullName, eventType.Name);
                        throw;
                    }
                }
            }
        }
    }
}
