using Pokok.BuildingBlocks.Domain.Events;

namespace Pokok.BuildingBlocks.Domain.Abstractions
{
    /// <summary>
    /// Contract for aggregate roots that collect and expose domain events.
    /// Events are accumulated during aggregate operations and cleared after dispatch.
    /// </summary>
    public interface IAggregateRoot
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}
