using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Evaluator
{
    /// <summary>
    /// Evaluates specifications by chaining all registered <see cref="ISpecificationHandler{T}"/>
    /// implementations (filter, order, include, paging) against an <see cref="IQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class SpecificationEvaluator<T> where T : class
    {
        private readonly IEnumerable<ISpecificationHandler<T>> _handlers;

        public SpecificationEvaluator(IEnumerable<ISpecificationHandler<T>> handlers)
        {
            _handlers = handlers;
        }

        public IQueryable<T> GetQuery(IQueryable<T> query, ISpecification<T> specification)
        {
            return _handlers.Aggregate(query, (current, handler) => handler.Apply(current, specification));
        }
    }
}
