using System.Linq.Expressions;

namespace Pokok.BuildingBlocks.Persistence.Abstractions
{
    /// <summary>
    /// Generic repository interface for CRUD operations.
    /// Changes are queued in the EF Core change tracker and persisted only when <see cref="IUnitOfWork.CompleteAsync"/> is called.
    /// </summary>
    /// <typeparam name="T">The entity type managed by this repository.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Gets an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The entity if found; otherwise, <c>null</c>.</returns>
        Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all entities of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>All entities in the repository.</returns>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds entities matching the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>Entities matching the predicate.</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the single entity matching the predicate, or <c>null</c> if none found.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The matching entity, or <c>null</c>.</returns>
        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds an entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a collection of entities to the repository.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks an entity for removal from the repository.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Remove(T entity);

        /// <summary>
        /// Marks a collection of entities for removal from the repository.
        /// </summary>
        /// <param name="entities">The entities to remove.</param>
        void RemoveRange(IEnumerable<T> entities);

        /// <summary>
        /// Determines whether any entity matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns><c>true</c> if a matching entity exists; otherwise, <c>false</c>.</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the count of entities matching the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The number of matching entities.</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
