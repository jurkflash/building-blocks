using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;
using Pokok.BuildingBlocks.Persistence.Specifications.Extensions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Core
{
    /// <summary>
    /// Negates a specification using logical NOT. Matches entities the inner specification would reject.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class NotSpecification<T> : BaseSpecification<T>
    {
        /// <summary>
        /// Initializes a new instance that negates the specified specification.
        /// </summary>
        /// <param name="spec">The specification to negate.</param>
        public NotSpecification(ISpecification<T> spec)
                : base(spec.ToExpression().Not())
        {
        }
    }
}
