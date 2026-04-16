using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pokok.BuildingBlocks.Outbox
{
    /// <summary>
    /// Default <see cref="IOutboxMessageRepository"/> implementation using EF Core.
    /// Logs all operations at INFO/WARN levels for observability.
    /// </summary>
    public class OutboxMessageRepository : IOutboxMessageRepository
    {
        private readonly OutboxDbContext _dbContext;
        private readonly ILogger<OutboxMessageRepository> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="OutboxMessageRepository"/>.
        /// </summary>
        /// <param name="dbContext">The outbox database context.</param>
        /// <param name="logger">Logger for repository operations.</param>
        public OutboxMessageRepository(OutboxDbContext dbContext, ILogger<OutboxMessageRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            await _dbContext.OutboxMessages.AddAsync(message, cancellationToken);
            _logger.LogInformation("Added outbox message. Id: {MessageId}, Type: {MessageType}", message.Id, message.Type);
        }

        /// <inheritdoc />
        public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int maxCount, CancellationToken cancellationToken = default)
        {
            var messages = await _dbContext.OutboxMessages
                .Where(m => m.ProcessedOnUtc == null)
                .OrderBy(m => m.OccurredOnUtc)
                .Take(maxCount)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Fetched {Count} unprocessed outbox messages.", messages.Count);
            return messages;
        }

        /// <inheritdoc />
        public async Task MarkAsProcessedAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var message = await _dbContext.OutboxMessages.FindAsync(new object[] { id }, cancellationToken);
            if (message is not null)
            {
                message.MarkAsProcessed();
                _logger.LogInformation("Marked outbox message as processed. Id: {MessageId}", id);
            }
            else
            {
                _logger.LogWarning("Failed to find outbox message to mark as processed. Id: {MessageId}", id);
            }
        }

        /// <inheritdoc />
        public async Task MarkAsFailedAsync(Guid id, string error, CancellationToken cancellationToken = default)
        {
            var message = await _dbContext.OutboxMessages.FindAsync(new object[] { id }, cancellationToken);
            if (message is not null)
            {
                message.MarkAsFailed(error);
                _logger.LogWarning("Marked outbox message as failed. Id: {MessageId}, Error: {Error}", id, error);
            }
            else
            {
                _logger.LogWarning("Failed to find outbox message to mark as failed. Id: {MessageId}", id);
            }
        }

        /// <inheritdoc />
        public async Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Saved changes to outbox.");
        }
    }
}
