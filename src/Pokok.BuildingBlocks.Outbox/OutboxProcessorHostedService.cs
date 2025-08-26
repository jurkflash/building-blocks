using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pokok.BuildingBlocks.Outbox
{
    public class OutboxProcessorHostedService<TDbContext> : BackgroundService
        where TDbContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxProcessorHostedService<TDbContext>> _logger;
        private readonly TimeSpan _interval;

        public OutboxProcessorHostedService(
            IServiceProvider serviceProvider,
            ILogger<OutboxProcessorHostedService<TDbContext>> logger,
            TimeSpan? interval = null)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _interval = interval ?? TimeSpan.FromSeconds(10);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox Processor started for {DbContext}.", typeof(TDbContext).Name);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
                    var outbox = dbContext.Set<OutboxMessage>();
                    var publisher = scope.ServiceProvider.GetRequiredService<IOutboxMessagePublisher>();

                    var messages = await outbox
                        .Where(m => m.ProcessedOnUtc == null)
                        .OrderBy(m => m.OccurredOnUtc)
                        .Take(10)
                        .ToListAsync(stoppingToken);

                    foreach (var message in messages)
                    {
                        try
                        {
                            await publisher.PublishAsync(message.Type.Value, message.Payload, stoppingToken);
                            message.MarkAsProcessed();
                        }
                        catch (Exception ex)
                        {
                            message.MarkAsFailed(ex.Message);
                            _logger.LogError(ex, "Failed to process outbox message {Id}", message.Id);
                        }
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error in OutboxProcessorHostedService");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
