namespace Pokok.BuildingBlocks.Cqrs.Abstractions
{
    /// <summary>
    /// Marker interface for query messages in the CQRS pattern.
    /// Queries represent read operations that return a result of type <typeparamref name="TResponse"/>.
    /// Query handlers should be side-effect free.
    /// </summary>
    /// <typeparam name="TResponse">The type of the result produced by handling this query.</typeparam>
    public interface IQuery<TResponse> { }
}
