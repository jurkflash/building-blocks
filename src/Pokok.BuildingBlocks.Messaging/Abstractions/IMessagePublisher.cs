namespace Pokok.BuildingBlocks.Messaging.Abstractions
{
    public interface IMessagePublisher
    {
        Task PublishAsync(string messageType, string payload, CancellationToken cancellationToken);
    }
}
