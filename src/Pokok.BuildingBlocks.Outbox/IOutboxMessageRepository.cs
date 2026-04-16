namespace Pokok.BuildingBlocks.Outbox
{
    /// <summary>
    /// Repository abstraction for outbox message persistence.
    /// Provides operations to add, query unprocessed, and mark messages as processed or failed.
    /// </summary>
    public interface IOutboxMessageRepository
    {
        /// <summary>
        /// Adds a new outbox message to the repository.
        /// </summary>
        /// <param name="message">The outbox message to add.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves unprocessed outbox messages up to the specified count, ordered by occurrence time.
        /// </summary>
        /// <param name="maxCount">The maximum number of messages to retrieve.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A list of unprocessed <see cref="OutboxMessage"/> instances.</returns>
        Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int maxCount, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks the specified outbox message as successfully processed.
        /// </summary>
        /// <param name="id">The identifier of the message to mark as processed.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task MarkAsProcessedAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks the specified outbox message as failed with the given error.
        /// </summary>
        /// <param name="id">The identifier of the message to mark as failed.</param>
        /// <param name="error">The error message describing the failure.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task MarkAsFailedAsync(Guid id, string error, CancellationToken cancellationToken = default);

        /// <summary>
        /// Persists all pending changes to the outbox store.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CompleteAsync(CancellationToken cancellationToken = default);
    }
}
