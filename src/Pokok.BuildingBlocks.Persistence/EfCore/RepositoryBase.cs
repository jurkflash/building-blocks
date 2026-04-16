using Microsoft.EntityFrameworkCore;
using Pokok.BuildingBlocks.Persistence.Abstractions;
using System.Linq.Expressions;

namespace Pokok.BuildingBlocks.Persistence.Base
{
    /// <summary>
    /// Generic repository base with a strongly-typed <typeparamref name="TContext"/> reference.
    /// Inherit from this when you need access to your specific DbContext type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type managed by this repository.</typeparam>
    /// <typeparam name="TContext">The specific <see cref="DbContext"/> type.</typeparam>
    public abstract class RepositoryBase<TEntity, TContext> : RepositoryBase<TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        protected new TContext Context { get; }

        protected RepositoryBase(TContext context) : base(context)
        {
            Context = context;
        }
    }

    /// <summary>
    /// Abstract repository providing standard CRUD operations backed by EF Core.
    /// Changes are tracked but not persisted until <see cref="IUnitOfWork.CompleteAsync"/> is called.
    /// </summary>
    /// <typeparam name="T">The entity type managed by this repository.</typeparam>
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly DbContext Context;

        protected RepositoryBase(DbContext context)
        {
            Context = context;
        }

        protected DbSet<T> DbSet => Context.Set<T>();

        /// <inheritdoc />
        public virtual async Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet.FindAsync(new object?[] { id }, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet.ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbSet.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
        }

        /// <inheritdoc />
        public virtual void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        /// <inheritdoc />
        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }

        /// <inheritdoc />
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await DbSet.AnyAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await DbSet.CountAsync(predicate, cancellationToken);

        }
    }
}
