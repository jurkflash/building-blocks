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
        /// <summary>
        /// Initializes a new instance that combines two specifications using logical AND.
        /// </summary>
        /// <param name="left">The first specification.</param>
        /// <param name="right">The second specification.</param>
        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
            : base(left.ToExpression().AndAlso(right.ToExpression()))
        {
        }
    }
}
