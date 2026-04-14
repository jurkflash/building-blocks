using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Handlers
{
    /// <summary>
    /// Applies a specification's filter criteria to an <see cref="IQueryable{T}"/> using <c>Where</c>.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class FilteringSpecificationHandler<T> : ISpecificationHandler<T>
    {
        public IQueryable<T> Apply(IQueryable<T> query, ISpecification<T> specification)
        {
            var predicate = specification.ToExpression();
            return predicate != null ? query.Where(predicate) : query;
        }
    }
}
