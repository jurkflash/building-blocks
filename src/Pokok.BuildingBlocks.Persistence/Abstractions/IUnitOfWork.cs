namespace Pokok.BuildingBlocks.Persistence.Abstractions
{
    /// <summary>
    /// Atomically saves all pending changes and dispatches domain events from aggregate roots.
    /// Call <see cref="CompleteAsync"/> to persist repository changes — without it, nothing is saved.
    /// </summary>
    public interface IUnitOfWork
    {
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}
