using System.Linq.Expressions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Contracts
{
    /// <summary>
    /// Specification contract for eager-loading related entities via EF Core <c>Include</c>.
    /// </summary>
    /// <typeparam name="T">The root entity type.</typeparam>
    public interface IIncludeSpecification<T> : ISpecification<T>
    {
        List<Expression<Func<T, object>>> Includes { get; }
    }
}
