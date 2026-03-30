namespace Pokok.BuildingBlocks.Messaging.Abstractions
{
    /// <summary>
    /// Abstracts the subscription/consumption side of a message broker.
    /// Symmetric counterpart to <see cref="IMessagePublisher"/>.
    /// </summary>
    public interface IMessageConsumer
    {
        /// <summary>
        /// Subscribes to messages on the specified <paramref name="queueName"/> and
        /// invokes <paramref name="handler"/> for each one.
        /// </summary>
        Task SubscribeAsync(
            string queueName,
            string routingKey,
            Func<string, string, CancellationToken, Task> handler,
            CancellationToken cancellationToken = default);
    }
}
