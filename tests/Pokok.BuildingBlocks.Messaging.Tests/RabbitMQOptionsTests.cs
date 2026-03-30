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
    public void Constructor_WithDefaults_SetsDefaultVirtualHost()
    {
        var options = new RabbitMQOptions();

        Assert.Equal("/", options.VirtualHost);
    }

    [Fact]
    public void Constructor_WithDefaults_SetsDefaultConsumerChannelCapacity()
    {
        var options = new RabbitMQOptions();

        Assert.Equal(256, options.ConsumerChannelCapacity);
    }

    [Fact]
    public void HostName_WhenSet_ReturnsExpectedValue()
    {
        var options = new RabbitMQOptions { HostName = "rabbitmq-host" };

        Assert.Equal("rabbitmq-host", options.HostName);
    }

    [Fact]
    public void VirtualHost_WhenSet_ReturnsExpectedValue()
    {
        var options = new RabbitMQOptions { VirtualHost = "/myapp" };

        Assert.Equal("/myapp", options.VirtualHost);
    }

    [Fact]
    public void ConsumerChannelCapacity_WhenSet_ReturnsExpectedValue()
    {
        var options = new RabbitMQOptions { ConsumerChannelCapacity = 64 };

        Assert.Equal(64, options.ConsumerChannelCapacity);
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

public class IMessageConsumerTests
{
    [Fact]
    public async Task SubscribeAsync_WhenCalled_InvokesConsumer()
    {
        var consumer = Substitute.For<IMessageConsumer>();

        static Task Handler(string type, string payload, CancellationToken ct) => Task.CompletedTask;

        await consumer.SubscribeAsync("order.queue", "order.*", Handler, CancellationToken.None);

        await consumer.Received(1).SubscribeAsync(
            "order.queue",
            "order.*",
            Handler,
            Arg.Any<CancellationToken>());
    }
}

