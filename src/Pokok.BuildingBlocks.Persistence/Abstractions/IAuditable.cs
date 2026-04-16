namespace Pokok.BuildingBlocks.Persistence.Abstractions
{
    /// <summary>
    /// Marks entities for automatic audit trail population.
    /// <see cref="DbContextBase"/> sets these fields during <c>SaveChangesAsync</c> based on entity state.
    /// </summary>
    public interface IAuditable
    {
        /// <summary>
        /// Gets or sets the UTC timestamp when the entity was created.
        /// </summary>
        DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created the entity.
        /// </summary>
        string? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp when the entity was last modified.
        /// </summary>
        DateTime? ModifiedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last modified the entity.
        /// </summary>
        string? ModifiedBy { get; set; }
    }
}
