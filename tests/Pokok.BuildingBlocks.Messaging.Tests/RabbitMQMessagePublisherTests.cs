using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pokok.BuildingBlocks.Messaging.RabbitMQ;
using RabbitMQ.Client;
using Xunit;

namespace Pokok.BuildingBlocks.Messaging.RabbitMQ;

public class RabbitMQMessagePublisherTests
{
    [Fact]
    public async Task PublishAsync_WhenChannelSucceeds_LogsSuccessfully()
    {
        var channel = Substitute.For<IChannel>();
        var connection = Substitute.For<IRabbitMQConnection>();
        connection.CreateChannelAsync().Returns(channel);

        var publisher = new RabbitMQMessagePublisher(
            connection,
            NullLogger<RabbitMQMessagePublisher>.Instance,
            "test.exchange");

        await publisher.PublishAsync("order.created", "{\"id\":1}", CancellationToken.None);

        await connection.Received(1).CreateChannelAsync();
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
}
