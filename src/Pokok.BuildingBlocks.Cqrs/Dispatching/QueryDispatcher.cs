using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pokok.BuildingBlocks.Cqrs.Abstractions;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching
{
    /// <summary>
    /// Dispatches queries to their registered <see cref="IQueryHandler{TQuery, TResponse}"/>
    /// via the dependency injection service provider. Logs dispatch and completion at DEBUG level.
    /// </summary>
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<QueryDispatcher> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDispatcher"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider used to resolve query handlers.</param>
        /// <param name="logger">The logger instance.</param>
        public QueryDispatcher(IServiceProvider serviceProvider, ILogger<QueryDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Dispatches the specified query to its registered handler.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="query">The query to dispatch.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The result of handling the query.</returns>
        public async Task<TResult> DispatchAsync<TQuery, TResult>(
            TQuery query,
            CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>
        {
            _logger.LogDebug("Dispatching query of type {QueryType}", typeof(TQuery).Name);

            var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
            var result = await handler.HandleAsync(query, cancellationToken);

            _logger.LogDebug("Query {QueryType} handled successfully", typeof(TQuery).Name);
            return result;
        }
    }
}
