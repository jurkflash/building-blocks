using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;
using Pokok.BuildingBlocks.Persistence.Specifications.Extensions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Core
{
    /// <summary>
    /// Combines two specifications using logical OR. At least one criterion must be satisfied.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class OrSpecification<T> : BaseSpecification<T>
    {
        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        : base(left.ToExpression().OrElse(right.ToExpression()))
        {
        }
    }
}
