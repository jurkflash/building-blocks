using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Handlers
{
    /// <summary>
    /// Applies pagination (skip/take) from a specification to an <see cref="IQueryable{T}"/>.
    /// Only applies when <see cref="IPagingSpecification.IsPagingEnabled"/> is <c>true</c>.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class PagingSpecificationHandler<T> : ISpecificationHandler<T>
    {
        public IQueryable<T> Apply(IQueryable<T> query, ISpecification<T> specification)
        {
            if (specification is IPagingSpecification pagingSpec && pagingSpec.IsPagingEnabled)
            {
                query = query.Skip(pagingSpec.Skip).Take(pagingSpec.Take);
            }
            return query;
        }
    }
}
