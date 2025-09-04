using Microsoft.EntityFrameworkCore;
using Pokok.BuildingBlocks.Persistence.Abstractions;
using System.Linq.Expressions;

namespace Pokok.BuildingBlocks.Persistence.Base
{
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

    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly DbContext Context;

        protected RepositoryBase(DbContext context)
        {
            Context = context;
        }

        protected DbSet<T> DbSet => Context.Set<T>();

        public virtual async Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet.FindAsync(new object?[] { id }, cancellationToken: cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet.ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public virtual async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbSet.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
        }

        public virtual void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await DbSet.AnyAsync(predicate, cancellationToken);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await DbSet.CountAsync(predicate, cancellationToken);

        }
    }
}
