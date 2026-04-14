namespace Pokok.BuildingBlocks.Persistence.Abstractions
{
    /// <summary>
    /// Marker interface for persistable entities with a <see cref="Guid"/> identifier.
    /// </summary>
    public interface IEntity
    {
        Guid Id { get; }
    }
}
