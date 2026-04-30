using Pokok.BuildingBlocks.Persistence.Abstractions;

namespace Pokok.BuildingBlocks.Persistence.Entities
{
    /// <summary>
    /// Convenience base class for persistable entities that implements <see cref="IEntity"/>,
    /// <see cref="IAuditable"/>, and <see cref="ISoftDeletable"/>. Provides an auto-generated <see cref="Guid"/> Id.
    /// </summary>
    public abstract class EntityBase : IEntity, IAuditable, ISoftDeletable
    {
        /// <summary>
        /// Gets the unique identifier for the entity.
        /// </summary>
#if NET9_0_OR_GREATER
        public Guid Id { get; protected set; } = Guid.CreateVersion7();
#else
        public Guid Id { get; protected set; } = Guid.NewGuid();
#endif

        /// <summary>
        /// Gets or sets the UTC timestamp when the entity was created.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created the entity.
        /// </summary>
        public string? CreatedBy { get; set; } 

        /// <summary>
        /// Gets or sets the UTC timestamp when the entity was last modified.
        /// </summary>
        public DateTime? ModifiedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last modified the entity.
        /// </summary>
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been soft-deleted.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp when the entity was soft-deleted.
        /// </summary>
        public DateTime? DeletedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who deleted the entity.
        /// </summary>
        public string? DeletedBy { get; set; }
    }
}
