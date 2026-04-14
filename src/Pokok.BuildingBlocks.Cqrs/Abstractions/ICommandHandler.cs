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
        Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}
