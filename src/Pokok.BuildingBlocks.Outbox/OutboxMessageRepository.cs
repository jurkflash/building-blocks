using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pokok.BuildingBlocks.Outbox
{
    public class OutboxMessageRepository : IOutboxMessageRepository
    {
        private readonly OutboxDbContext _dbContext;
        private readonly ILogger<OutboxMessageRepository> _logger;

        public OutboxMessageRepository(OutboxDbContext dbContext, ILogger<OutboxMessageRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            await _dbContext.OutboxMessages.AddAsync(message, cancellationToken);
            _logger.LogInformation("Added outbox message. Id: {MessageId}, Type: {MessageType}", message.Id, message.Type);
        }

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

        public async Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Saved changes to outbox.");
        }
    }
}
