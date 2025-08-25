using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pokok.BuildingBlocks.Cqrs.Abstractions;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<QueryDispatcher> _logger;

        public QueryDispatcher(IServiceProvider serviceProvider, ILogger<QueryDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

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
