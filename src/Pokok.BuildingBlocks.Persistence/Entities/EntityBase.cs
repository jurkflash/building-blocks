using Pokok.BuildingBlocks.Persistence.Abstractions;

namespace Pokok.BuildingBlocks.Persistence.Entities
{
    /// <summary>
    /// Convenience base class for persistable entities that implements <see cref="IEntity"/>,
    /// <see cref="IAuditable"/>, and <see cref="ISoftDeletable"/>. Provides an auto-generated <see cref="Guid"/> Id.
    /// </summary>
    public abstract class EntityBase : IEntity, IAuditable, ISoftDeletable
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        // Auditable
        public DateTime CreatedAtUtc { get; set; }
        public string? CreatedBy { get; set; } 
        public DateTime? ModifiedAtUtc { get; set; }
        public string? ModifiedBy { get; set; }

        // Soft delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public string? DeletedBy { get; set; }
    }
}
