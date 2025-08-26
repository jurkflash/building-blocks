namespace Pokok.BuildingBlocks.Outbox
{
    public interface IOutboxMessagePublisher
    {
        Task PublishAsync(string messageType, string payload, CancellationToken cancellationToken);
    }
}
