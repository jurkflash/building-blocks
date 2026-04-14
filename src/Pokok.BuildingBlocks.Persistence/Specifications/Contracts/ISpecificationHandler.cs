namespace Pokok.BuildingBlocks.Persistence.Specifications.Contracts
{
    /// <summary>
    /// Applies a specification to an <see cref="IQueryable{T}"/> to build the final query.
    /// Multiple handlers (filter, order, paging, include) are chained together by <see cref="SpecificationEvaluator{T}"/>.
    /// </summary>
    /// <typeparam name="T">The entity type to query.</typeparam>
    public interface ISpecificationHandler<T>
    {
        IQueryable<T> Apply(IQueryable<T> query, ISpecification<T> specification);
    }
}
