using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;
using System.Linq.Expressions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Core
{
    /// <summary>
    /// Abstract base for specifications providing a filter criteria expression,
    /// optional ordering, includes, and pagination support.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        /// <summary>
        /// Gets the filter criteria expression for this specification.
        /// </summary>
        public Expression<Func<T, bool>> Criteria { get; }

        /// <summary>
        /// Converts this specification to a LINQ expression tree.
        /// </summary>
        /// <returns>The filter criteria as an expression.</returns>
        public virtual Expression<Func<T, bool>> ToExpression() => Criteria;

        /// <summary>
        /// Compiles this specification's criteria to an in-memory predicate function.
        /// </summary>
        /// <returns>A compiled predicate delegate.</returns>
        public Func<T, bool> ToPredicate() => Criteria.Compile();

        /// <summary>
        /// Gets the number of items to skip, or <c>null</c> if paging is not applied.
        /// </summary>
        public int? Skip { get; private set; }

        /// <summary>
        /// Gets the number of items to take, or <c>null</c> if paging is not applied.
        /// </summary>
        public int? Take { get; private set; }

        protected void ApplyPaging(int skip, int take)
        {
            if (skip < 0) throw new ArgumentOutOfRangeException(nameof(skip));
            if (take <= 0) throw new ArgumentOutOfRangeException(nameof(take));

            Skip = skip;
            Take = take;
        }
    }
}
