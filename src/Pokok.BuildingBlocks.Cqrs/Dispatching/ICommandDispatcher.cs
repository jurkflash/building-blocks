using Pokok.BuildingBlocks.Cqrs.Abstractions;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching
{
    /// <summary>
    /// Dispatches a command to its registered handler and returns the result.
    /// Throws <see cref="InvalidOperationException"/> if no handler is registered for the command type.
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        /// Dispatches a command to its registered handler and returns the result.
        /// </summary>
        /// <typeparam name="TCommand">The command type.</typeparam>
        /// <typeparam name="TResponse">The result type.</typeparam>
        /// <param name="command">The command to dispatch.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The result of handling the command.</returns>
        Task<TResponse> DispatchAsync<TCommand, TResponse>(
            TCommand command,
            CancellationToken cancellationToken = default)
            where TCommand : ICommand<TResponse>;
    }
}
