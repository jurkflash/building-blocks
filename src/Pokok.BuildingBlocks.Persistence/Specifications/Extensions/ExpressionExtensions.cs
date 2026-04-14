using System.Linq.Expressions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Extensions
{
    /// <summary>
    /// Extension methods for combining LINQ expression trees using <c>AndAlso</c>, <c>OrElse</c>, and <c>Not</c> operators.
    /// Used by the specification combinators (<see cref="AndSpecification{T}"/>, <see cref="OrSpecification{T}"/>, <see cref="NotSpecification{T}"/>).
    /// </summary>
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            var parameter = left.Parameters[0];
            var rightBody = new ParameterReplacer(right.Parameters[0], parameter).Visit(right.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left.Body, rightBody!), parameter);
        }

        public static Expression<Func<T, bool>> OrElse<T>(
            this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            var parameter = left.Parameters[0];
            var rightBody = new ParameterReplacer(right.Parameters[0], parameter).Visit(right.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left.Body, rightBody!), parameter);
        }

        public static Expression<Func<T, bool>> Not<T>(
            this Expression<Func<T, bool>> expression)
        {
            var parameter = expression.Parameters[0];
            var notBody = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(notBody, parameter);
        }

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _source;
            private readonly ParameterExpression _target;

            public ParameterReplacer(ParameterExpression source, ParameterExpression target)
            {
                _source = source;
                _target = target;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _source ? _target : base.VisitParameter(node);
            }
        }
    }
}
