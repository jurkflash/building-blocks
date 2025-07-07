using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Evaluator
{
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
