using System.Linq.Expressions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Contracts
{
    /// <summary>
    /// Specification contract for sorting query results in ascending or descending order.
    /// </summary>
    /// <typeparam name="T">The entity type to sort.</typeparam>
    public interface IOrderSpecification<T> : ISpecification<T>
    {
        /// <summary>
        /// Gets the ascending order expression, or <c>null</c> if not specified.
        /// </summary>
        Expression<Func<T, object>>? OrderBy { get; }

        /// <summary>
        /// Gets the descending order expression, or <c>null</c> if not specified.
        /// </summary>
        Expression<Func<T, object>>? OrderByDescending { get; }
    }
}
