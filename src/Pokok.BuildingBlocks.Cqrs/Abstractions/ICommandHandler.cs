namespace Pokok.BuildingBlocks.Cqrs.Abstractions
{
    /// <summary>
    /// Handles a command of type <typeparamref name="TCommand"/> and returns a result of type <typeparamref name="TResponse"/>.
    /// Register implementations via <c>CqrsRegistrationExtensions.AddCommandHandler</c>.
    /// </summary>
    /// <typeparam name="TCommand">The command type to handle.</typeparam>
    /// <typeparam name="TResponse">The type of the result returned after handling.</typeparam>
    public interface ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        /// <summary>
        /// Handles the specified command and returns a result.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The result of handling the command.</returns>
        Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}
