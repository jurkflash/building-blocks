namespace Pokok.BuildingBlocks.Domain.Events
{
    /// <summary>
    /// Represents something meaningful that happened in the domain.
    /// Domain events are immutable facts with a UTC timestamp.
    /// </summary>
    public interface IDomainEvent
    {
        /// <summary>
        /// Gets the UTC date and time when this event occurred.
        /// </summary>
        DateTime OccurredOn { get; }
    }
}
