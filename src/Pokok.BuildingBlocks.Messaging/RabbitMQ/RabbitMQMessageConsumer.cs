using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace Pokok.BuildingBlocks.Messaging.RabbitMQ
{
    /// <summary>
    /// A <see cref="BackgroundService"/> that consumes messages from a RabbitMQ queue and dispatches
    /// them to an <see cref="IRabbitMQMessageHandler{T}"/>.
    /// <para>
    /// Delivery from RabbitMQ is decoupled from processing via a bounded
    /// <see cref="Channel{T}"/> (signal). The RabbitMQ consumer callback only enqueues the raw
    /// delivery into the channel; a separate reader loop in <see cref="ExecuteAsync"/> deserialises
    /// and handles each message. This avoids event-handler entanglement and provides natural
    /// back-pressure when the handler is slower than the broker.
    /// </para>
    /// </summary>
    public class RabbitMQMessageConsumer<T> : BackgroundService
    {
        private readonly IRabbitMQConnection _connection;
        private readonly IRabbitMQMessageHandler<T> _handler;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly string _routingKey;
        private readonly int _channelCapacity;
        private readonly ILogger<RabbitMQMessageConsumer<T>> _logger;
        private IChannel? _rabbitChannel;

        public RabbitMQMessageConsumer(
            IRabbitMQConnection connection,
            IRabbitMQMessageHandler<T> handler,
            IOptions<RabbitMQOptions> options,
            ILogger<RabbitMQMessageConsumer<T>> logger,
            string queueName,
            string routingKey,
            string exchangeName = "pokok.exchange")
        {
            _connection = connection;
            _handler = handler;
            _logger = logger;
            _queueName = queueName;
            _routingKey = routingKey;
            _exchangeName = exchangeName;
            _channelCapacity = options.Value.ConsumerChannelCapacity;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Starting RabbitMQ consumer for queue '{Queue}', routing key '{RoutingKey}', exchange '{Exchange}'",
                _queueName, _routingKey, _exchangeName);

            // --- Set up the in-process signal channel ---
            // The RabbitMQ delivery callback writes raw deliveries here; the reader loop below
            // processes them. BoundedChannelFullMode.Wait provides back-pressure: the broker
            // callback blocks (async) until capacity is available, naturally throttling ingest.
            var deliveryChannel = Channel.CreateBounded<BasicDeliverEventArgs>(
                new BoundedChannelOptions(_channelCapacity)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleWriter = false,
                    SingleReader = true,
                });

            // --- Configure the RabbitMQ channel and consumer ---
            _rabbitChannel = await _connection.CreateChannelAsync();
            _logger.LogDebug("RabbitMQ channel created.");

            await _rabbitChannel.ExchangeDeclareAsync(
                exchange: _exchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            await _rabbitChannel.QueueDeclareAsync(
                _queueName, durable: true, exclusive: false, autoDelete: false,
                cancellationToken: cancellationToken);

            await _rabbitChannel.QueueBindAsync(
                queue: _queueName,
                exchange: _exchangeName,
                routingKey: _routingKey,
                cancellationToken: cancellationToken);

            _logger.LogDebug(
                "Queue '{Queue}' bound to exchange '{Exchange}' with routing key '{RoutingKey}'.",
                _queueName, _exchangeName, _routingKey);

            // The consumer callback is intentionally thin: it just signals the in-process channel.
            var consumer = new AsyncEventingBasicConsumer(_rabbitChannel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                _logger.LogDebug("Delivery received on queue '{Queue}', enqueuing to signal channel.", _queueName);
                await deliveryChannel.Writer.WriteAsync(ea, cancellationToken);
            };

            await _rabbitChannel.BasicConsumeAsync(
                _queueName, autoAck: false, consumer, cancellationToken);

            _logger.LogInformation("RabbitMQ consumer started for queue '{Queue}'.", _queueName);

            // --- Reader loop: process deliveries from the signal channel ---
            await foreach (var ea in deliveryChannel.Reader.ReadAllAsync(cancellationToken))
            {
                await ProcessDeliveryAsync(ea, cancellationToken);
            }
        }

        private async Task ProcessDeliveryAsync(BasicDeliverEventArgs ea, CancellationToken cancellationToken)
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            _logger.LogDebug("Processing message from queue '{Queue}'.", _queueName);

            try
            {
                var message = JsonSerializer.Deserialize<T>(json);

                if (message is null)
                {
                    _logger.LogWarning("Deserialized message is null — skipping.");
                }
                else
                {
                    _logger.LogDebug("Dispatching message of type {MessageType}.", typeof(T).Name);
                    await _handler.HandleAsync(message, cancellationToken);
                    _logger.LogDebug("Message handled successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling message from queue '{Queue}'.", _queueName);
            }

            try
            {
                if (_rabbitChannel is not null)
                    await _rabbitChannel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to acknowledge message (delivery tag {Tag}).", ea.DeliveryTag);
            }
        }

        public override void Dispose()
        {
            _logger.LogInformation("Disposing RabbitMQ consumer for queue '{Queue}'.", _queueName);
            _rabbitChannel?.Dispose();
            base.Dispose();
        }
    }
}
