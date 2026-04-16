using Pokok.BuildingBlocks.Domain.Events;

namespace Pokok.BuildingBlocks.Domain.Abstractions
{
    /// <summary>
    /// Contract for aggregate roots that collect and expose domain events.
    /// Events are accumulated during aggregate operations and cleared after dispatch.
    /// </summary>
    public interface IAggregateRoot
    {
        /// <summary>
        /// Gets the collection of domain events raised by this aggregate.
        /// </summary>
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

        /// <summary>
        /// Removes all domain events from this aggregate.
        /// </summary>
        void ClearDomainEvents();
    }
}
