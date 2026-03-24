using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Pokok.BuildingBlocks.Domain.SharedKernel.Enums;
using Xunit;

namespace Pokok.BuildingBlocks.Outbox;

public class OutboxDbContextTests
{
    private static OutboxDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<OutboxDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new OutboxDbContext(options);
    }

    [Fact]
    public async Task OutboxMessages_CanAddAndRetrieveMessage()
    {
        using var context = CreateContext();
        var message = new OutboxMessage(OutboxMessageType.Email, "{\"to\":\"test@example.com\"}", "test-app");

        context.OutboxMessages.Add(message);
        await context.SaveChangesAsync();

        var saved = await context.OutboxMessages.FirstAsync();
        Assert.Equal(message.Id, saved.Id);
        Assert.Equal("test-app", saved.SourceApp);
    }
}

public class OutboxMessageRepositoryTests
{
    private static (OutboxDbContext context, OutboxMessageRepository repo) CreateRepo()
    {
        var options = new DbContextOptionsBuilder<OutboxDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new OutboxDbContext(options);
        var repo = new OutboxMessageRepository(context, NullLogger<OutboxMessageRepository>.Instance);
        return (context, repo);
    }

    [Fact]
    public async Task AddAsync_WithMessage_PersistsMessage()
    {
        var (context, repo) = CreateRepo();
        var message = new OutboxMessage(OutboxMessageType.Email, "payload", "app");

        await repo.AddAsync(message);
        await repo.CompleteAsync();

        Assert.Single(context.OutboxMessages);
    }

    [Fact]
    public async Task GetUnprocessedMessagesAsync_ReturnsOnlyUnprocessed()
    {
        var (_, repo) = CreateRepo();
        var unprocessed = new OutboxMessage(OutboxMessageType.Email, "payload1", "app");
        var processed = new OutboxMessage(OutboxMessageType.Email, "payload2", "app");
        processed.MarkAsProcessed();

        await repo.AddAsync(unprocessed);
        await repo.AddAsync(processed);
        await repo.CompleteAsync();

        var result = await repo.GetUnprocessedMessagesAsync(10);

        Assert.Single(result);
        Assert.Equal(unprocessed.Id, result[0].Id);
    }

    [Fact]
    public async Task GetUnprocessedMessagesAsync_RespectsMaxCount()
    {
        var (_, repo) = CreateRepo();
        for (var i = 0; i < 5; i++)
        {
            await repo.AddAsync(new OutboxMessage(OutboxMessageType.Email, $"payload{i}", "app"));
        }
        await repo.CompleteAsync();

        var result = await repo.GetUnprocessedMessagesAsync(3);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task MarkAsProcessedAsync_WithExistingId_MarksMessageAsProcessed()
    {
        var (_, repo) = CreateRepo();
        var message = new OutboxMessage(OutboxMessageType.Email, "payload", "app");
        await repo.AddAsync(message);
        await repo.CompleteAsync();

        await repo.MarkAsProcessedAsync(message.Id);
        await repo.CompleteAsync();

        var unprocessed = await repo.GetUnprocessedMessagesAsync(10);
        Assert.Empty(unprocessed);
    }

    [Fact]
    public async Task MarkAsProcessedAsync_WithNonExistingId_DoesNotThrow()
    {
        var (_, repo) = CreateRepo();

        await repo.MarkAsProcessedAsync(Guid.NewGuid());
    }

    [Fact]
    public async Task MarkAsFailedAsync_WithExistingId_SetsError()
    {
        var (context, repo) = CreateRepo();
        var message = new OutboxMessage(OutboxMessageType.Email, "payload", "app");
        await repo.AddAsync(message);
        await repo.CompleteAsync();

        await repo.MarkAsFailedAsync(message.Id, "Connection refused");

        var saved = context.OutboxMessages.First();
        Assert.Equal("Connection refused", saved.Error);
    }

    [Fact]
    public async Task MarkAsFailedAsync_WithNonExistingId_DoesNotThrow()
    {
        var (_, repo) = CreateRepo();

        await repo.MarkAsFailedAsync(Guid.NewGuid(), "error");
    }

    [Fact]
    public void OutboxOptions_DefaultInterval_IsTenSeconds()
    {
        var options = new OutboxOptions();

        Assert.Equal(TimeSpan.FromSeconds(10), options.Interval);
    }
}
