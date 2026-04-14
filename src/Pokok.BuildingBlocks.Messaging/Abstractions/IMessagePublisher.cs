namespace Pokok.BuildingBlocks.Messaging.Abstractions
{
    /// <summary>
    /// Transport-agnostic abstraction for publishing messages to a message broker.
    /// Use this interface (not concrete implementations) for testability and transport swapping.
    /// </summary>
    public interface IMessagePublisher
    {
        Task PublishAsync(string messageType, string payload, CancellationToken cancellationToken);
    }
}
