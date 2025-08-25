using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pokok.BuildingBlocks.Cqrs.Abstractions;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CommandDispatcher> _logger;

        public CommandDispatcher(IServiceProvider serviceProvider, ILogger<CommandDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

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
