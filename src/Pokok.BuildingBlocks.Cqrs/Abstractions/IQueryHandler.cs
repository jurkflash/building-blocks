namespace Pokok.BuildingBlocks.Cqrs.Abstractions
{
    /// <summary>
    /// Handles a query of type <typeparamref name="TQuery"/> and returns a result of type <typeparamref name="TResponse"/>.
    /// Query handlers should be side-effect free. Register via <c>CqrsRegistrationExtensions.AddQueryHandler</c>.
    /// </summary>
    /// <typeparam name="TQuery">The query type to handle.</typeparam>
    /// <typeparam name="TResponse">The type of the result returned after handling.</typeparam>
    public interface IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
    {
        Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken);
    }
}
