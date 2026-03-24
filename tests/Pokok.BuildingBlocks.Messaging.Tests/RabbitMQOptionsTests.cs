using NSubstitute;
using Pokok.BuildingBlocks.Messaging.Abstractions;
using Pokok.BuildingBlocks.Messaging.RabbitMQ;
using Xunit;

namespace Pokok.BuildingBlocks.Messaging;

public class RabbitMQOptionsTests
{
    [Fact]
    public void Constructor_WithDefaults_SetsDefaultPort()
    {
        var options = new RabbitMQOptions();

        Assert.Equal(5672, options.Port);
    }

    [Fact]
    public void Constructor_WithDefaults_SetsDefaultCredentials()
    {
        var options = new RabbitMQOptions();

        Assert.Equal("guest", options.UserName);
        Assert.Equal("guest", options.Password);
    }

    [Fact]
    public void HostName_WhenSet_ReturnsExpectedValue()
    {
        var options = new RabbitMQOptions { HostName = "rabbitmq-host" };

        Assert.Equal("rabbitmq-host", options.HostName);
    }
}

public class IMessagePublisherTests
{
    [Fact]
    public async Task PublishAsync_WithValidPayload_InvokesPublisher()
    {
        var publisher = Substitute.For<IMessagePublisher>();

        await publisher.PublishAsync("order.created", "{\"id\":1}", CancellationToken.None);

        await publisher.Received(1).PublishAsync(
            "order.created",
            "{\"id\":1}",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PublishAsync_WithCancellationToken_PassesTokenToPublisher()
    {
        var publisher = Substitute.For<IMessagePublisher>();
        var cts = new CancellationTokenSource();

        await publisher.PublishAsync("order.created", "{}", cts.Token);

        await publisher.Received(1).PublishAsync(
            Arg.Any<string>(), Arg.Any<string>(), cts.Token);
    }
}
