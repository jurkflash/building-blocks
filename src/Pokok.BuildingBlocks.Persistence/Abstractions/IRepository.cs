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
        Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
