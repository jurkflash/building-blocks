using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;
using Pokok.BuildingBlocks.Persistence.Specifications.Extensions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Core
{
    /// <summary>
    /// Combines two specifications using logical AND. Both criteria must be satisfied.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class AndSpecification<T> : BaseSpecification<T>
    {
        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
            : base(left.ToExpression().AndAlso(right.ToExpression()))
        {
        }
    }
}
