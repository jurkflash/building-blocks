using Microsoft.Extensions.DependencyInjection;
using Pokok.BuildingBlocks.Domain.Events;

namespace Pokok.BuildingBlocks.Cqrs.Events
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in domainEvents)
            {
                var eventType = domainEvent.GetType();

                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
                var handlers = _serviceProvider.GetServices(handlerType);

                foreach (var handler in handlers)
                {
                    var handleMethod = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.Handle));

                    if (handleMethod is not null)
                    {
                        var task = (Task?)handleMethod.Invoke(handler, new object[] { domainEvent, cancellationToken });
                        if (task != null)
                            await task.ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
