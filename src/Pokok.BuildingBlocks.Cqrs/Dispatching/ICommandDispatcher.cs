using Pokok.BuildingBlocks.Cqrs.Abstractions;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching
{
    /// <summary>
    /// Dispatches a command to its registered handler and returns the result.
    /// Throws <see cref="InvalidOperationException"/> if no handler is registered for the command type.
    /// </summary>
    public interface ICommandDispatcher
    {
        Task<TResponse> DispatchAsync<TCommand, TResponse>(
            TCommand command,
            CancellationToken cancellationToken = default)
            where TCommand : ICommand<TResponse>;
    }
}
