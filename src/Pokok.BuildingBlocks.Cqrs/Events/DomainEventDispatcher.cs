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
            string correlationId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();

            foreach (var domainEvent in domainEvents)
            {
                var eventType = domainEvent.GetType();
                _logger.LogDebug("CorrelationId: {CorrelationId} - Dispatching domain event: {EventType}", correlationId, eventType.Name);

                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
                var handlers = _serviceProvider.GetServices(handlerType);

                if (!handlers.Cast<object>().Any())
                {
                    _logger.LogWarning("CorrelationId: {CorrelationId} - No handlers found for domain event: {EventType}", correlationId, eventType.Name);
                    continue;
                }

                foreach (var handler in handlers)
                {
                    var handleMethod = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.Handle));
                    if (handleMethod is null)
                    {
                        _logger.LogWarning("CorrelationId: {CorrelationId} - Handle method not found for {HandlerType}", correlationId, handler.GetType().FullName);
                        continue;
                    }

                    try
                    {
                        _logger.LogDebug("CorrelationId: {CorrelationId} - Invoking handler {HandlerType} for event {EventType}", correlationId, handler.GetType().FullName, eventType.Name);

                        var task = (Task?)handleMethod.Invoke(handler, new object[] { domainEvent, cancellationToken });
                        if (task != null)
                            await task.ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "CorrelationId: {CorrelationId} - Exception in handler {HandlerType} for domain event {EventType}", correlationId, handler.GetType().FullName, eventType.Name);
                        throw;
                    }
                }
            }
        }
    }
}
