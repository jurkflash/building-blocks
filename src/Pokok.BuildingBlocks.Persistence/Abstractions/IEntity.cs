namespace Pokok.BuildingBlocks.Persistence.Abstractions
{
    /// <summary>
    /// Marker interface for persistable entities with a <see cref="Guid"/> identifier.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Gets the unique identifier of the entity.
        /// </summary>
        Guid Id { get; }
    }
}
