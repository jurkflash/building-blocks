using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pokok.BuildingBlocks.Outbox
{
    /// <summary>
    /// Extension methods for registering the outbox processor and repository in the DI container.
    /// Call <see cref="AddOutboxProcessor{TDbContext}"/> to register <see cref="IOutboxMessageRepository"/>
    /// and the <see cref="OutboxProcessorHostedService{TDbContext}"/> background service.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOutboxProcessor<TDbContext>(
            this IServiceCollection services)
            where TDbContext : DbContext
        {
            services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();

            services.AddHostedService(sp =>
            {
                var options = sp.GetRequiredService<IOptions<OutboxOptions>>();
                var logger = sp.GetRequiredService<ILogger<OutboxProcessorHostedService<TDbContext>>>();

                return new OutboxProcessorHostedService<TDbContext>(options, sp, logger);
            });

            return services;
        }
    }
}
