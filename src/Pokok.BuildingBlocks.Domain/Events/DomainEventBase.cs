namespace Pokok.BuildingBlocks.Domain.Events
{
    /// <summary>
    /// Convenience base class for domain events.
    /// Automatically sets <see cref="OccurredOn"/> to <see cref="DateTime.UtcNow"/> at construction.
    /// </summary>
    public abstract class DomainEventBase : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
