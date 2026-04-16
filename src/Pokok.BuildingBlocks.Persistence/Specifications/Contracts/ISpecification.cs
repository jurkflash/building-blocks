using System.Linq.Expressions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Contracts
{
    /// <summary>
    /// Core specification contract for encapsulating query logic as a composable boolean expression.
    /// Specifications can be combined using <c>And</c>, <c>Or</c>, and <c>Not</c> operators.
    /// </summary>
    /// <typeparam name="T">The entity type to evaluate.</typeparam>
    public interface ISpecification<T>
    {
        /// <summary>
        /// Converts this specification to a LINQ expression tree.
        /// </summary>
        /// <returns>The filter criteria as an expression.</returns>
        Expression<Func<T, bool>> ToExpression();
    }
}
