namespace Pokok.BuildingBlocks.Domain.Events
{

    /// <summary>
    /// Handles a specific type of domain event.
    /// Multiple handlers per event type are supported and invoked sequentially by the dispatcher.
    /// </summary>
    /// <typeparam name="TDomainEvent">The domain event type to handle.</typeparam>
    public interface IDomainEventHandler<in TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        /// <summary>
        /// Handles the specified domain event.
        /// </summary>
        /// <param name="domainEvent">The domain event to handle.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken);
    }
}
