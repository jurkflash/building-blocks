namespace Pokok.BuildingBlocks.Messaging.RabbitMQ
{
    /// <summary>
    /// Handles deserialized messages of type <typeparamref name="T"/> received from a RabbitMQ queue.
    /// Implement this interface for each message type you want to consume.
    /// </summary>
    /// <typeparam name="T">The message type to handle.</typeparam>
    public interface IRabbitMQMessageHandler<T>
    {
        Task HandleAsync(T message, CancellationToken cancellationToken = default);
    }
}
