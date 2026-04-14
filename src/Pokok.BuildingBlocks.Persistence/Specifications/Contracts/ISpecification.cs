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
        Expression<Func<T, bool>> ToExpression();
    }
}
