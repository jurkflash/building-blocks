using System.Text.Json;
using ModelContextProtocol;
using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Dispatching;
using Pokok.BuildingBlocks.Cqrs.Validation;

namespace Pokok.BuildingBlocks.Mcp.Tools
{
    /// <summary>
    /// Abstract base class for MCP server tools that dispatch a CQRS query.
    /// Subclasses should be decorated with [McpServerToolType] and expose at least one
    /// [McpServerTool] method that builds a <typeparamref name="TQuery"/> and calls
    /// <see cref="ExecuteAsync"/>.
    /// </summary>
    /// <typeparam name="TQuery">The CQRS query type.</typeparam>
    /// <typeparam name="TResult">The query result type.</typeparam>
    public abstract class McpQueryToolBase<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Dispatches <paramref name="query"/> via <paramref name="dispatcher"/> and returns
        /// the result as a JSON string. <see cref="ValidationException"/> is translated into a
        /// descriptive <see cref="McpException"/> so that AI clients receive a structured error
        /// instead of an unhandled exception.
        /// </summary>
        protected static async Task<string> ExecuteAsync(
            IQueryDispatcher dispatcher,
            TQuery query,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await dispatcher.DispatchAsync<TQuery, TResult>(query, cancellationToken);
                return JsonSerializer.Serialize(result);
            }
            catch (ValidationException ex)
            {
                throw new McpException($"Validation failed: {string.Join("; ", ex.Errors)}");
            }
        }
    }
}
