namespace Pokok.BuildingBlocks.Messaging.Abstractions
{
    /// <summary>
    /// Transport-agnostic abstraction for publishing messages to a message broker.
    /// Use this interface (not concrete implementations) for testability and transport swapping.
    /// </summary>
    public interface IMessagePublisher
    {
        /// <summary>
        /// Publishes a message with the specified type and payload to the message broker.
        /// </summary>
        /// <param name="messageType">The message type identifier used for routing.</param>
        /// <param name="payload">The serialized message payload.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous publish operation.</returns>
        Task PublishAsync(string messageType, string payload, CancellationToken cancellationToken);
    }
}
