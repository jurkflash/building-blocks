namespace Pokok.BuildingBlocks.Cqrs.Abstractions
{
    /// <summary>
    /// Marker interface for command messages in the CQRS pattern.
    /// Commands represent write operations that produce a result of type <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TResponse">The type of the result produced by handling this command.</typeparam>
    public interface ICommand<TResponse> { }
}
