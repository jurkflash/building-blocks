using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Pokok.BuildingBlocks.Messaging.RabbitMQ
{
    public class RabbitMQMessageConsumer<T> : BackgroundService
    {
        private readonly IRabbitMQConnection _connection;
        private readonly IRabbitMQMessageHandler<T> _handler;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly string _routingKey;
        private readonly ILogger<RabbitMQMessageConsumer<T>> _logger;
        private IChannel? _channel;

        public RabbitMQMessageConsumer(
            IRabbitMQConnection connection,
            IRabbitMQMessageHandler<T> handler,
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
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting RabbitMQ consumer for queue '{Queue}' and routing key '{RoutingKey}' on exchange '{Exchange}'", _queueName, _routingKey, _exchangeName);

            _channel = await _connection.CreateChannelAsync();
            _logger.LogDebug("RabbitMQ channel created.");

            await _channel.ExchangeDeclareAsync(
                exchange: _exchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);
            _logger.LogDebug("Exchange '{Exchange}' declared.", _exchangeName);

            await _channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false);
            _logger.LogDebug("Queue '{Queue}' declared.", _queueName);

            await _channel.QueueBindAsync(queue: _queueName, exchange: _exchangeName, routingKey: _routingKey, cancellationToken: cancellationToken);
            _logger.LogDebug("Queue '{Queue}' bound to exchange '{Exchange}' with routing key '{RoutingKey}'", _queueName, _exchangeName, _routingKey);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogDebug("Message received from queue '{Queue}': {MessageBody}", _queueName, json);

                try
                {
                    var message = JsonSerializer.Deserialize<T>(json);

                    if (message == null)
                    {
                        _logger.LogWarning("Deserialized message is null. Skipping.");
                        return;
                    }

                    _logger.LogDebug("Handling message of type {MessageType}", typeof(T).Name);
                    await _handler.HandleAsync(message, cancellationToken);
                    _logger.LogDebug("Message handled successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while handling message.");
                }

                try
                {
                    await _channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
                    _logger.LogDebug("Message acknowledged.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to acknowledge message.");
                }
            };

            await _channel.BasicConsumeAsync(_queueName, autoAck: false, consumer, cancellationToken);
            _logger.LogInformation("RabbitMQ consumer started for queue '{Queue}'", _queueName);
        }

        public override void Dispose()
        {
            _logger.LogInformation("Disposing RabbitMQ consumer for queue '{Queue}'", _queueName);
            _channel?.Dispose();
            base.Dispose();
        }
    }
}
