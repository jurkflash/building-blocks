using Microsoft.EntityFrameworkCore;

namespace Pokok.BuildingBlocks.Outbox
{
    public class OutboxMessageRepository : IOutboxMessageRepository
    {
        private readonly OutboxDbContext _dbContext;

        public OutboxMessageRepository(OutboxDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            await _dbContext.OutboxMessages.AddAsync(message, cancellationToken);
        }

        public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int maxCount, CancellationToken cancellationToken = default)
        {
            return await _dbContext.OutboxMessages
                .Where(m => m.ProcessedOnUtc == null)
                .OrderBy(m => m.OccurredOnUtc)
                .Take(maxCount)
                .ToListAsync(cancellationToken);
        }

        public async Task MarkAsProcessedAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var message = await _dbContext.OutboxMessages.FindAsync(new object[] { id }, cancellationToken);
            if (message is not null)
            {
                message.MarkAsProcessed();
            }
        }

        public async Task MarkAsFailedAsync(Guid id, string error, CancellationToken cancellationToken = default)
        {
            var message = await _dbContext.OutboxMessages.FindAsync(new object[] { id }, cancellationToken);
            if (message is not null)
            {
                message.MarkAsFailed(error);
            }
        }

        public async Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
