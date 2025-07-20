namespace Pokok.BuildingBlocks.Outbox
{
    public interface IOutboxMessageRepository
    {
        Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);
        Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int maxCount, CancellationToken cancellationToken = default);
        Task MarkAsProcessedAsync(Guid id, CancellationToken cancellationToken = default);
        Task MarkAsFailedAsync(Guid id, string error, CancellationToken cancellationToken = default);
        Task CompleteAsync(CancellationToken cancellationToken = default);
    }
}
