using System.Linq.Expressions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Contracts
{
    /// <summary>
    /// Specification contract for projecting entities to a different result shape via a selector expression.
    /// </summary>
    /// <typeparam name="T">The source entity type.</typeparam>
    /// <typeparam name="TResult">The projected result type.</typeparam>
    public interface IProjectionSpecification<T, TResult>
    {
        /// <summary>
        /// Gets the projection selector expression.
        /// </summary>
        Expression<Func<T, TResult>> Selector { get; }
    }
}
