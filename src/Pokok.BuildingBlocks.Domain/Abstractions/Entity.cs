using Pokok.BuildingBlocks.Domain.Events;

namespace Pokok.BuildingBlocks.Domain.Abstractions
{
    /// <summary>
    /// Base class for domain entities with identity-based equality.
    /// Two entities are equal if and only if their <see cref="Id"/> values are equal.
    /// </summary>
    /// <typeparam name="TId">The type of the entity's identifier.</typeparam>
    public abstract class Entity<TId>
    {
        /// <summary>
        /// Gets the unique identifier for this entity.
        /// </summary>
        public TId Id { get; protected set; } = default!;

        protected Entity() { }

        protected Entity(TId id)
        {
            Id = id;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Entity<TId> other) return false;
            return EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public override int GetHashCode() => Id?.GetHashCode() ?? 0;
    }
}
