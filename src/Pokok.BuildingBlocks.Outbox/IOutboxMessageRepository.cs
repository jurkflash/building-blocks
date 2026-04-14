namespace Pokok.BuildingBlocks.Outbox
{
    /// <summary>
    /// Repository abstraction for outbox message persistence.
    /// Provides operations to add, query unprocessed, and mark messages as processed or failed.
    /// </summary>
    public interface IOutboxMessageRepository
    {
        Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);
        Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int maxCount, CancellationToken cancellationToken = default);
        Task MarkAsProcessedAsync(Guid id, CancellationToken cancellationToken = default);
        Task MarkAsFailedAsync(Guid id, string error, CancellationToken cancellationToken = default);
        Task CompleteAsync(CancellationToken cancellationToken = default);
    }
}
