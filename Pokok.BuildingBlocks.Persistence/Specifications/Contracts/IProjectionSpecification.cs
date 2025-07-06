using System.Linq.Expressions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Contracts
{
    public interface IProjectionSpecification<T, TResult>
    {
        Expression<Func<T, TResult>> Selector { get; }
    }
}
