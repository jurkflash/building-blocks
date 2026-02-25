using Pokok.BuildingBlocks.Domain.Events;

namespace Pokok.BuildingBlocks.Domain.Abstractions
{
    public interface IAggregateRoot
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}
