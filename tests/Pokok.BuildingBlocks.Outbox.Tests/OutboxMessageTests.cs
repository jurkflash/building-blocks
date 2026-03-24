using Pokok.BuildingBlocks.Domain.SharedKernel.Enums;
using Xunit;

namespace Pokok.BuildingBlocks.Outbox;

public class OutboxMessageTests
{
    [Fact]
    public void Constructor_WithValidArguments_CreatesOutboxMessage()
    {
        var message = new OutboxMessage(
            OutboxMessageType.Email,
            "{\"to\":\"user@example.com\"}",
            "identity-service");

        Assert.Equal(OutboxMessageType.Email, message.Type);
        Assert.Equal("{\"to\":\"user@example.com\"}", message.Payload);
        Assert.Equal("identity-service", message.SourceApp);
        Assert.Null(message.ProcessedOnUtc);
        Assert.Null(message.Error);
    }

    [Fact]
    public void Constructor_WithNullType_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new OutboxMessage(null!, "payload", "app"));
    }

    [Fact]
    public void Constructor_WithNullPayload_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new OutboxMessage(OutboxMessageType.Email, null!, "app"));
    }

    [Fact]
    public void Constructor_WithNullSourceApp_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new OutboxMessage(OutboxMessageType.Email, "payload", null!));
    }

    [Fact]
    public void MarkAsProcessed_WhenCalled_SetsProcessedOnUtc()
    {
        var message = new OutboxMessage(OutboxMessageType.Email, "payload", "app");

        message.MarkAsProcessed();

        Assert.NotNull(message.ProcessedOnUtc);
        Assert.Null(message.Error);
    }

    [Fact]
    public void MarkAsFailed_WithErrorMessage_SetsError()
    {
        var message = new OutboxMessage(OutboxMessageType.Email, "payload", "app");

        message.MarkAsFailed("Connection timeout.");

        Assert.Equal("Connection timeout.", message.Error);
        Assert.Null(message.ProcessedOnUtc);
    }

    [Fact]
    public void Id_WhenCreated_HasNonEmptyGuid()
    {
        var message = new OutboxMessage(OutboxMessageType.Email, "payload", "app");

        Assert.NotEqual(Guid.Empty, message.Id);
    }

    [Fact]
    public void Constructor_WithCustomOccurredOnUtc_UsesProvidedValue()
    {
        var occurredOn = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        var message = new OutboxMessage(OutboxMessageType.Email, "payload", "app", occurredOn);

        Assert.Equal(occurredOn, message.OccurredOnUtc);
    }
}
