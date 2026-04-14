namespace Pokok.BuildingBlocks.Persistence.Abstractions
{
    /// <summary>
    /// Marks entities for soft delete. When deleted, <see cref="DbContextBase"/> automatically converts
    /// the hard delete to a soft delete by setting <see cref="IsDeleted"/> to <c>true</c>.
    /// Soft-deleted entities are excluded from queries via global query filters.
    /// </summary>
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAtUtc { get; set; }
        string? DeletedBy { get; set; }
    }
}
