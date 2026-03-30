using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pokok.BuildingBlocks.Messaging.RabbitMQ;
using RabbitMQ.Client;
using Xunit;

namespace Pokok.BuildingBlocks.Messaging.RabbitMQ;

public class RabbitMQMessagePublisherTests
{
    private static IChannel OpenChannel()
    {
        var ch = Substitute.For<IChannel>();
        ch.IsClosed.Returns(false);
        return ch;
    }

    [Fact]
    public async Task PublishAsync_WhenChannelSucceeds_CreatesChannelOnce()
    {
        var channel = OpenChannel();
        var connection = Substitute.For<IRabbitMQConnection>();
        connection.CreateChannelAsync().Returns(channel);

        var publisher = new RabbitMQMessagePublisher(
            connection,
            NullLogger<RabbitMQMessagePublisher>.Instance,
            "test.exchange");

        // Two publishes — channel should be created only once and reused.
        await publisher.PublishAsync("order.created", "{\"id\":1}", CancellationToken.None);
        await publisher.PublishAsync("order.updated", "{\"id\":1}", CancellationToken.None);

        await connection.Received(1).CreateChannelAsync();
    }

    [Fact]
    public async Task PublishAsync_WhenChannelIsClosed_RecreatesChannel()
    {
        var channel = Substitute.For<IChannel>();
        channel.IsClosed.Returns(false); // open for the first publish

        var connection = Substitute.For<IRabbitMQConnection>();
        connection.CreateChannelAsync().Returns(channel);

        var publisher = new RabbitMQMessagePublisher(
            connection,
            NullLogger<RabbitMQMessagePublisher>.Instance,
            "test.exchange");

        // First publish uses the open channel.
        await publisher.PublishAsync("order.created", "{}", CancellationToken.None);
        await connection.Received(1).CreateChannelAsync();

        // Simulate the channel being closed between publishes.
        channel.IsClosed.Returns(true);

        // Second publish should detect the closed channel and recreate it.
        await publisher.PublishAsync("order.updated", "{}", CancellationToken.None);

        await connection.Received(2).CreateChannelAsync();
    }

    [Fact]
    public async Task PublishAsync_WhenConnectionThrows_RethrowsException()
    {
        var connection = Substitute.For<IRabbitMQConnection>();
        connection.CreateChannelAsync().Throws(new InvalidOperationException("Connection failed"));

        var publisher = new RabbitMQMessagePublisher(
            connection,
            NullLogger<RabbitMQMessagePublisher>.Instance);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => publisher.PublishAsync("order.created", "{}", CancellationToken.None));
    }

    [Fact]
    public async Task PublishAsync_WhenBasicPublishThrows_InvalidatesChannelAndRethrows()
    {
        var channel = OpenChannel();
        // BasicPublishAsync returns ValueTask — use Returns to simulate failure.
        channel.BasicPublishAsync(
                exchange: Arg.Any<string>(),
                routingKey: Arg.Any<string>(),
                mandatory: Arg.Any<bool>(),
                basicProperties: Arg.Any<BasicProperties>(),
                body: Arg.Any<ReadOnlyMemory<byte>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(new ValueTask(Task.FromException(new InvalidOperationException("publish failed"))));

        var connection = Substitute.For<IRabbitMQConnection>();
        connection.CreateChannelAsync().Returns(channel);

        var publisher = new RabbitMQMessagePublisher(
            connection,
            NullLogger<RabbitMQMessagePublisher>.Instance,
            "test.exchange");

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => publisher.PublishAsync("order.created", "{}", CancellationToken.None));

        // After the failure the channel is invalidated; a second publish must recreate it.
        var channel2 = OpenChannel();
        connection.CreateChannelAsync().Returns(channel2);
        await publisher.PublishAsync("order.created", "{}", CancellationToken.None);

        await connection.Received(2).CreateChannelAsync();
    }
}
