using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Pokok.BuildingBlocks.Messaging.RabbitMQ;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Pokok.BuildingBlocks.Messaging.RabbitMQ;

// Must be public so NSubstitute can create a generic proxy for IRabbitMQMessageHandler<OrderMessage>.
public record OrderMessage(int Id);

/// <summary>
/// Validates the signal-channel pipeline in <see cref="RabbitMQMessageConsumer{T}"/>:
/// messages enqueued by the RabbitMQ delivery callback are picked up and dispatched
/// by the reader loop in <c>ExecuteAsync</c>.
/// </summary>
public class RabbitMQMessageConsumerTests
{
    private static IOptions<RabbitMQOptions> DefaultOptions(int capacity = 16) =>
        Options.Create(new RabbitMQOptions { ConsumerChannelCapacity = capacity });

    private static ReadOnlyMemory<byte> Serialize<T>(T message) =>
        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

    [Fact]
    public async Task ExecuteAsync_ValidMessage_IsDispatchedToHandler()
    {
        // Arrange
        var received = new List<OrderMessage>();
        var handler = Substitute.For<IRabbitMQMessageHandler<OrderMessage>>();
        handler
            .HandleAsync(Arg.Do<OrderMessage>(m => received.Add(m)), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var rabbitChannel = Substitute.For<IChannel>();
        var connection = Substitute.For<IRabbitMQConnection>();
        connection.CreateChannelAsync().Returns(rabbitChannel);

        IHostedService consumer = new RabbitMQMessageConsumer<OrderMessage>(
            connection, handler, DefaultOptions(),
            NullLogger<RabbitMQMessageConsumer<OrderMessage>>.Instance,
            "order.queue", "order.created");

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        // Act — start the background service
        await consumer.StartAsync(cts.Token);

        // Give the service a moment to set up the RabbitMQ infrastructure.
        await Task.Delay(80, CancellationToken.None);

        // Retrieve the AsyncEventingBasicConsumer registered via BasicConsumeAsync.
        var registeredConsumer = await CaptureRegisteredConsumerAsync(rabbitChannel);
        Assert.NotNull(registeredConsumer);

        // Simulate a delivery from RabbitMQ.
        await registeredConsumer!.HandleBasicDeliverAsync(
            "test-consumer", 1, false, "pokok.exchange", "order.created",
            new BasicProperties(), Serialize(new OrderMessage(42)));

        // Wait briefly for the signal channel reader to process it.
        await Task.Delay(150, CancellationToken.None);

        await consumer.StopAsync(CancellationToken.None);

        // Assert
        Assert.Single(received);
        Assert.Equal(42, received[0].Id);
    }

    [Fact]
    public async Task ExecuteAsync_NullDeserialisedMessage_DoesNotCallHandler()
    {
        var handler = Substitute.For<IRabbitMQMessageHandler<OrderMessage>>();
        var rabbitChannel = Substitute.For<IChannel>();
        var connection = Substitute.For<IRabbitMQConnection>();
        connection.CreateChannelAsync().Returns(rabbitChannel);

        IHostedService consumer = new RabbitMQMessageConsumer<OrderMessage>(
            connection, handler, DefaultOptions(),
            NullLogger<RabbitMQMessageConsumer<OrderMessage>>.Instance,
            "order.queue", "order.created");

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await consumer.StartAsync(cts.Token);
        await Task.Delay(80, CancellationToken.None);

        var registeredConsumer = await CaptureRegisteredConsumerAsync(rabbitChannel);

        // Send a payload that deserialises to null (bare JSON null).
        await registeredConsumer!.HandleBasicDeliverAsync(
            "test-consumer", 1, false, "pokok.exchange", "order.created",
            new BasicProperties(), Encoding.UTF8.GetBytes("null"));

        await Task.Delay(150, CancellationToken.None);
        await consumer.StopAsync(CancellationToken.None);

        await handler.DidNotReceive().HandleAsync(Arg.Any<OrderMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_HandlerThrows_AcknowledgesAndContinues()
    {
        var handler = Substitute.For<IRabbitMQMessageHandler<OrderMessage>>();
        handler
            .HandleAsync(Arg.Any<OrderMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new InvalidOperationException("handler failure")));

        var rabbitChannel = Substitute.For<IChannel>();
        var connection = Substitute.For<IRabbitMQConnection>();
        connection.CreateChannelAsync().Returns(rabbitChannel);

        IHostedService consumer = new RabbitMQMessageConsumer<OrderMessage>(
            connection, handler, DefaultOptions(),
            NullLogger<RabbitMQMessageConsumer<OrderMessage>>.Instance,
            "order.queue", "order.created");

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await consumer.StartAsync(cts.Token);
        await Task.Delay(80, CancellationToken.None);

        var registeredConsumer = await CaptureRegisteredConsumerAsync(rabbitChannel);

        await registeredConsumer!.HandleBasicDeliverAsync(
            "test-consumer", 7, false, "pokok.exchange", "order.created",
            new BasicProperties(), Serialize(new OrderMessage(1)));

        await Task.Delay(200, CancellationToken.None);
        await consumer.StopAsync(CancellationToken.None);

        // Even though the handler threw, BasicAckAsync should still be attempted.
        await rabbitChannel.Received().BasicAckAsync(7, false, Arg.Any<CancellationToken>());
    }

    // ---- helper ----

    /// <summary>
    /// Polls the NSubstitute call log on <paramref name="rabbitChannel"/> until
    /// <c>BasicConsumeAsync</c> is recorded and returns its <see cref="IAsyncBasicConsumer"/> argument.
    /// </summary>
    private static async Task<IAsyncBasicConsumer?> CaptureRegisteredConsumerAsync(
        IChannel rabbitChannel,
        int maxWaitMs = 3000)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(maxWaitMs);
        while (DateTime.UtcNow < deadline)
        {
            var call = rabbitChannel.ReceivedCalls()
                .FirstOrDefault(c => c.GetMethodInfo().Name == nameof(IChannel.BasicConsumeAsync));
            if (call is not null)
                return call.GetArguments()[6] as IAsyncBasicConsumer;

            await Task.Delay(20);
        }
        return null;
    }
}
