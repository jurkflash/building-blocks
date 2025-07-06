using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;
using Pokok.BuildingBlocks.Persistence.Specifications.Extensions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Core
{
    public class NotSpecification<T> : BaseSpecification<T>
    {
        public NotSpecification(ISpecification<T> spec)
                : base(spec.ToExpression().Not())
        {
        }
    }
}
