namespace Pokok.BuildingBlocks.Persistence.Abstractions
{
    /// <summary>
    /// Atomically saves all pending changes and dispatches domain events from aggregate roots.
    /// Call <see cref="CompleteAsync"/> to persist repository changes — without it, nothing is saved.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Saves all pending changes and dispatches domain events.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}
