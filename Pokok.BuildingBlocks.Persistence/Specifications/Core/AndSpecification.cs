using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;
using Pokok.BuildingBlocks.Persistence.Specifications.Extensions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Core
{
    public class AndSpecification<T> : BaseSpecification<T>
    {
        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
            : base(left.ToExpression().AndAlso(right.ToExpression()))
        {
        }
    }
}
