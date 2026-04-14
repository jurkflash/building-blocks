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
        public NotSpecification(ISpecification<T> spec)
                : base(spec.ToExpression().Not())
        {
        }
    }
}
