using System.Linq.Expressions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Contracts
{
    public interface IFilterSpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
    }
}
