using System.Linq.Expressions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Contracts
{
    public interface IIncludeSpecification<T> : ISpecification<T>
    {
        List<Expression<Func<T, object>>> Includes { get; }
    }
}
