using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pokok.BuildingBlocks.Cqrs.Abstractions;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching
{
    /// <summary>
    /// Dispatches commands to their registered <see cref="ICommandHandler{TCommand, TResponse}"/>
    /// via the dependency injection service provider. Logs dispatch and completion at DEBUG level.
    /// </summary>
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CommandDispatcher> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDispatcher"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider used to resolve command handlers.</param>
        /// <param name="logger">The logger instance.</param>
        public CommandDispatcher(IServiceProvider serviceProvider, ILogger<CommandDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Dispatches the specified command to its registered handler.
        /// </summary>
        /// <typeparam name="TCommand">The command type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="command">The command to dispatch.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The result of handling the command.</returns>
        public async Task<TResult> DispatchAsync<TCommand, TResult>(
            TCommand command,
            CancellationToken cancellationToken = default)
            where TCommand : ICommand<TResult>
        {
            _logger.LogDebug("Dispatching command of type {CommandType}", typeof(TCommand).Name);

            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();

            var result = await handler.HandleAsync(command, cancellationToken);

            _logger.LogDebug("Command {CommandType} handled successfully", typeof(TCommand).Name);
            return result;
        }
    }
}
