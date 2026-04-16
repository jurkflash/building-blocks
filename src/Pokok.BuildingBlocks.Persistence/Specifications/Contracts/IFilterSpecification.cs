using System.Linq.Expressions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Contracts
{
    /// <summary>
    /// Specification contract providing a filter criteria expression for querying entities.
    /// </summary>
    /// <typeparam name="T">The entity type to filter.</typeparam>
    public interface IFilterSpecification<T>
    {
        /// <summary>
        /// Gets the filter criteria expression.
        /// </summary>
        Expression<Func<T, bool>> Criteria { get; }
    }
}
