using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Handlers
{
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
