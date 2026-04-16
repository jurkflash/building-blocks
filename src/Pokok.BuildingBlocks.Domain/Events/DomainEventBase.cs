namespace Pokok.BuildingBlocks.Domain.Events
{
    /// <summary>
    /// Convenience base class for domain events.
    /// Automatically sets <see cref="OccurredOn"/> to <see cref="DateTime.UtcNow"/> at construction.
    /// </summary>
    public abstract class DomainEventBase : IDomainEvent
    {
        /// <summary>
        /// Gets the UTC date and time when this event occurred.
        /// </summary>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
