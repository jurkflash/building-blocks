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

        /// <summary>
        /// Initializes a new instance of <see cref="SpecificationEvaluator{T}"/> with the specified handlers.
        /// </summary>
        /// <param name="handlers">The specification handlers to apply when evaluating queries.</param>
        public SpecificationEvaluator(IEnumerable<ISpecificationHandler<T>> handlers)
        {
            _handlers = handlers;
        }

        /// <summary>
        /// Applies all registered specification handlers to build the final query.
        /// </summary>
        /// <param name="query">The base queryable to apply the specification against.</param>
        /// <param name="specification">The specification containing query criteria.</param>
        /// <returns>The resulting <see cref="IQueryable{T}"/> with all handlers applied.</returns>
        public IQueryable<T> GetQuery(IQueryable<T> query, ISpecification<T> specification)
        {
            return _handlers.Aggregate(query, (current, handler) => handler.Apply(current, specification));
        }
    }
}
