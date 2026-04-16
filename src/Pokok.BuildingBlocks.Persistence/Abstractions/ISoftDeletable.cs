namespace Pokok.BuildingBlocks.Persistence.Abstractions
{
    /// <summary>
    /// Marks entities for soft delete. When deleted, <see cref="DbContextBase"/> automatically converts
    /// the hard delete to a soft delete by setting <see cref="IsDeleted"/> to <c>true</c>.
    /// Soft-deleted entities are excluded from queries via global query filters.
    /// </summary>
    public interface ISoftDeletable
    {
        /// <summary>
        /// Gets or sets a value indicating whether the entity has been soft-deleted.
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp when the entity was soft-deleted.
        /// </summary>
        DateTime? DeletedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who deleted the entity.
        /// </summary>
        string? DeletedBy { get; set; }
    }
}
