using System.Linq.Expressions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Contracts
{
    /// <summary>
    /// Specification contract for sorting query results in ascending or descending order.
    /// </summary>
    /// <typeparam name="T">The entity type to sort.</typeparam>
    public interface IOrderSpecification<T> : ISpecification<T>
    {
        Expression<Func<T, object>>? OrderBy { get; }
        Expression<Func<T, object>>? OrderByDescending { get; }
    }
}
