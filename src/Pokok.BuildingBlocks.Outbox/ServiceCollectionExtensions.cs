using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pokok.BuildingBlocks.Outbox
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOutboxProcessor<TDbContext>(
            this IServiceCollection services,
            TimeSpan? interval = null)
            where TDbContext : DbContext
        {
            services.AddHostedService(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<OutboxProcessorHostedService<TDbContext>>>();
                return new OutboxProcessorHostedService<TDbContext>(sp, logger, interval);
            });

            return services;
        }
    }
}
