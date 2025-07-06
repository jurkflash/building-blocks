using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Handlers
{
    public class OrderSpecificationHandler<T> : ISpecificationHandler<T>
    {
        public IQueryable<T> Apply(IQueryable<T> query, ISpecification<T> specification)
        {
            if (specification is IOrderSpecification<T> orderSpec)
            {
                if (orderSpec.OrderBy != null)
                {
                    query = query.OrderBy(orderSpec.OrderBy);
                }
                else if (orderSpec.OrderByDescending != null)
                {
                    query = query.OrderByDescending(orderSpec.OrderByDescending);
                }
            }
            return query;
        }
    }
}
