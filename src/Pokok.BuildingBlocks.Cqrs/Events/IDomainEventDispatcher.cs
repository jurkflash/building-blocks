using Pokok.BuildingBlocks.Domain.Events;

namespace Pokok.BuildingBlocks.Cqrs.Events
{
    /// <summary>
    /// Dispatches a collection of domain events to all registered handlers.
    /// Supports multiple handlers per event type.
    /// </summary>
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    }
}
