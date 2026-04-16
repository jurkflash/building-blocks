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
        /// <summary>
        /// Registers the outbox processor background service and message repository in the DI container.
        /// </summary>
        /// <typeparam name="TDbContext">The application's <see cref="DbContext"/> type containing the outbox table.</typeparam>
        /// <param name="services">The service collection to add services to.</param>
        /// <returns>The service collection for chaining.</returns>
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
