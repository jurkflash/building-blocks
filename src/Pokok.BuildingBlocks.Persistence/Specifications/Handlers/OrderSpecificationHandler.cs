using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Handlers
{
    /// <summary>
    /// Applies a specification's ordering expressions to an <see cref="IQueryable{T}"/> using <c>OrderBy</c>/<c>OrderByDescending</c>.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
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
