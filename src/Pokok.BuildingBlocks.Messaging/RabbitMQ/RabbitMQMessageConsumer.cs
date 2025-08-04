using Microsoft.Extensions.Hosting;
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
        private IChannel? _channel;

        public RabbitMQMessageConsumer(
            IRabbitMQConnection connection,
            IRabbitMQMessageHandler<T> handler,
            string queueName,
            string routingKey,
            string exchangeName = "pokok.exchange")
        {
            _connection = connection;
            _handler = handler;
            _queueName = queueName;
            _routingKey = routingKey;
            _exchangeName = exchangeName;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(
                exchange: _exchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            await _channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false);

            await _channel.QueueBindAsync(queue: _queueName, exchange: _exchangeName, routingKey: _routingKey, cancellationToken: cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<T>(json);

                if (message != null)
                    await _handler.HandleAsync(message, cancellationToken);

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync(_queueName, autoAck: false, consumer);
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            base.Dispose();
        }
    }
}
