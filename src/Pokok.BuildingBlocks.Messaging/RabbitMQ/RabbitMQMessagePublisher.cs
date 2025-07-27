using Microsoft.Extensions.Logging;
using Pokok.BuildingBlocks.Messaging.Abstractions;
using RabbitMQ.Client;
using System.Text;

namespace Pokok.BuildingBlocks.Messaging.RabbitMQ
{
    public class RabbitMQMessagePublisher : IMessagePublisher
    {
        private readonly IRabbitMQConnection _connection;
        private readonly ILogger<RabbitMQMessagePublisher> _logger;
        private readonly string _exchangeName;

        public RabbitMQMessagePublisher(
            IRabbitMQConnection connection,
            ILogger<RabbitMQMessagePublisher> logger,
            string exchangeName = "pokok.exchange")
        {
            _connection = connection;
            _logger = logger;
            _exchangeName = exchangeName;
        }

        public async Task PublishAsync(string messageType, string payload, CancellationToken cancellationToken)
        {
            try
            {
                await using var channel = await _connection.CreateChannelAsync();

                // Declare the exchange (optional if already declared externally)
                await channel.ExchangeDeclareAsync(
                    exchange: _exchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: cancellationToken);

                var body = Encoding.UTF8.GetBytes(payload);

                var properties = new BasicProperties();
                properties.ContentType = "application/json";
                properties.DeliveryMode = DeliveryModes.Persistent;

                var routingKey = messageType.ToString(); // Or use a custom routing strategy

                await channel.BasicPublishAsync(
                    exchange: _exchangeName,
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);

                _logger.LogInformation("Published message to RabbitMQ. Type: {MessageType}, RoutingKey: {RoutingKey}", messageType, routingKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message to RabbitMQ. Type: {MessageType}", messageType);
                throw;
            }
        }
    }
}
