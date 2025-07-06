using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;
using Pokok.BuildingBlocks.Persistence.Specifications.Extensions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Core
{
    public class OrSpecification<T> : BaseSpecification<T>
    {
        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        : base(left.ToExpression().OrElse(right.ToExpression()))
        {
        }
    }
}
