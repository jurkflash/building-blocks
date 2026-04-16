using Pokok.BuildingBlocks.Domain.Events;

namespace Pokok.BuildingBlocks.Cqrs.Events
{
    /// <summary>
    /// Dispatches a collection of domain events to all registered handlers.
    /// Supports multiple handlers per event type.
    /// </summary>
    public interface IDomainEventDispatcher
    {
        /// <summary>
        /// Dispatches domain events to all registered handlers.
        /// </summary>
        /// <param name="domainEvents">The domain events to dispatch.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    }
}
