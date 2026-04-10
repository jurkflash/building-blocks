using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pokok.BuildingBlocks.Cqrs.Dispatching;

namespace Pokok.BuildingBlocks.Mcp.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="IMcpServerBuilder"/> to integrate CQRS dispatchers.
    /// </summary>
    public static class McpBuilderExtensions
    {
        /// <summary>
        /// Registers <see cref="ICommandDispatcher"/> and <see cref="IQueryDispatcher"/> in the
        /// service collection so that MCP tool methods can resolve them via
        /// <see cref="IServiceProvider"/>. This is a no-op if the dispatchers are already
        /// registered by the application.
        /// </summary>
        public static IMcpServerBuilder WithCqrsDispatchers(this IMcpServerBuilder builder)
        {
            builder.Services.TryAddScoped<ICommandDispatcher, CommandDispatcher>();
            builder.Services.TryAddScoped<IQueryDispatcher, QueryDispatcher>();
            return builder;
        }
    }
}
