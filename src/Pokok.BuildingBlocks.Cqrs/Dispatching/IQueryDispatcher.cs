using Pokok.BuildingBlocks.Cqrs.Abstractions;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching
{
    /// <summary>
    /// Dispatches a query to its registered handler and returns the result.
    /// Throws <see cref="InvalidOperationException"/> if no handler is registered for the query type.
    /// </summary>
    public interface IQueryDispatcher
    {
        Task<TResponse> DispatchAsync<TQuery, TResponse>(
            TQuery query,
            CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResponse>;
    }
}
