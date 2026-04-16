using Pokok.BuildingBlocks.Cqrs.Abstractions;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching
{
    /// <summary>
    /// Dispatches a query to its registered handler and returns the result.
    /// Throws <see cref="InvalidOperationException"/> if no handler is registered for the query type.
    /// </summary>
    public interface IQueryDispatcher
    {
        /// <summary>
        /// Dispatches a query to its registered handler and returns the result.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <typeparam name="TResponse">The result type.</typeparam>
        /// <param name="query">The query to dispatch.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The result of handling the query.</returns>
        Task<TResponse> DispatchAsync<TQuery, TResponse>(
            TQuery query,
            CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResponse>;
    }
}
