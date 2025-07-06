using Pokok.BuildingBlocks.Persistence.Abstractions;

namespace Pokok.BuildingBlocks.Persistence.Entities
{
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
