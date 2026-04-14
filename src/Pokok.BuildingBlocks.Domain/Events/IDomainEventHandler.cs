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
        Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken);
    }
}
