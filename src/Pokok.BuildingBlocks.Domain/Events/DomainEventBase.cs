using Pokok.BuildingBlocks.Domain.Abstractions;

namespace Pokok.BuildingBlocks.Domain.Events
{
    public abstract class DomainEventBase : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
