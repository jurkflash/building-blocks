namespace Pokok.BuildingBlocks.Persistence.Abstractions
{
    /// <summary>
    /// Marks entities for automatic audit trail population.
    /// <see cref="DbContextBase"/> sets these fields during <c>SaveChangesAsync</c> based on entity state.
    /// </summary>
    public interface IAuditable
    {
        DateTime CreatedAtUtc { get; set; }
        string? CreatedBy { get; set; }
        DateTime? ModifiedAtUtc { get; set; }
        string? ModifiedBy { get; set; }
    }
}
