using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Pokok.BuildingBlocks.Domain.SharedKernel.Enums;
using Pokok.BuildingBlocks.Messaging.Abstractions;
using Xunit;

namespace Pokok.BuildingBlocks.Outbox;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddOutboxProcessor_RegistersOutboxMessageRepository()
    {
        var services = new ServiceCollection();
        services.AddOutboxProcessor<OutboxDbContext>();

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IOutboxMessageRepository));

        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AddOutboxProcessor_RegistersHostedService()
    {
        var services = new ServiceCollection();
        services.AddOutboxProcessor<OutboxDbContext>();

        var hostedDescriptors = services.Where(d => d.ServiceType.Name.Contains("IHostedService")).ToList();

        Assert.NotEmpty(hostedDescriptors);
    }
}

public class OutboxProcessorHostedServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WhenCancelledImmediately_ExitsWithoutProcessing()
    {
        var options = Options.Create(new OutboxOptions { Interval = TimeSpan.FromMilliseconds(50) });
        var logger = NullLogger<OutboxProcessorHostedService<OutboxDbContext>>.Instance;
        var serviceProvider = Substitute.For<IServiceProvider>();
        var scope = Substitute.For<IServiceScope>();
        var scopeFactory = Substitute.For<IServiceScopeFactory>();
        serviceProvider.GetService(typeof(IServiceScopeFactory)).Returns(scopeFactory);
        scopeFactory.CreateScope().Returns(scope);

        var dbContextOptions = new DbContextOptionsBuilder<OutboxDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var dbContext = new OutboxDbContext(dbContextOptions);
        var publisher = Substitute.For<IMessagePublisher>();

        scope.ServiceProvider.GetService(typeof(OutboxDbContext)).Returns(dbContext);
        scope.ServiceProvider.GetService(typeof(IMessagePublisher)).Returns(publisher);

        var service = new OutboxProcessorHostedService<OutboxDbContext>(options, serviceProvider, logger);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await service.StartAsync(cts.Token);
        await service.StopAsync(CancellationToken.None);

        service.Dispose();
    }
}
