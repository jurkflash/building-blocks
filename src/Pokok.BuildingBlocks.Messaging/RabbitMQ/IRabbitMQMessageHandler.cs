namespace Pokok.BuildingBlocks.Messaging.RabbitMQ
{
    /// <summary>
    /// Handles deserialized messages of type <typeparamref name="T"/> received from a RabbitMQ queue.
    /// Implement this interface for each message type you want to consume.
    /// </summary>
    /// <typeparam name="T">The message type to handle.</typeparam>
    public interface IRabbitMQMessageHandler<T>
    {
        /// <summary>
        /// Handles a received message of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="message">The deserialized message to handle.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous handling operation.</returns>
        Task HandleAsync(T message, CancellationToken cancellationToken = default);
    }
}
