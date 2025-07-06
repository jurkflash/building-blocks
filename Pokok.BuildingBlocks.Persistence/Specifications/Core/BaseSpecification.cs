using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;
using System.Linq.Expressions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Core
{
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>> Criteria { get; }

        public virtual Expression<Func<T, bool>> ToExpression() => Criteria;

        public Func<T, bool> ToPredicate() => Criteria.Compile();

        public int? Skip { get; private set; }
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
