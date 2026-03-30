using Microsoft.Extensions.Logging;
using Pokok.BuildingBlocks.Messaging.Abstractions;
using RabbitMQ.Client;
using System.Text;

namespace Pokok.BuildingBlocks.Messaging.RabbitMQ
{
    /// <summary>
    /// Publishes messages to a RabbitMQ topic exchange.
    /// <para>
    /// A single <see cref="IChannel"/> is created lazily and reused across calls.
    /// The exchange is declared once on first use. If the channel is closed (e.g. after a
    /// broker disconnect), it is transparently recreated on the next publish attempt.
    /// </para>
    /// </summary>
    public class RabbitMQMessagePublisher : IMessagePublisher, IAsyncDisposable
    {
        private readonly IRabbitMQConnection _connection;
        private readonly ILogger<RabbitMQMessagePublisher> _logger;
        private readonly string _exchangeName;
        private readonly SemaphoreSlim _channelLock = new(1, 1);
        private IChannel? _channel;

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
            var channel = await GetOrCreateChannelAsync(cancellationToken);

            try
            {
                var body = Encoding.UTF8.GetBytes(payload);

                var properties = new BasicProperties
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent,
                };

                await channel.BasicPublishAsync(
                    exchange: _exchangeName,
                    routingKey: messageType,
                    mandatory: true,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "Published message to RabbitMQ. Type: {MessageType}, Exchange: {Exchange}",
                    messageType, _exchangeName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message to RabbitMQ. Type: {MessageType}", messageType);

                // Invalidate channel so it is recreated on next attempt.
                await InvalidateChannelAsync();

                throw;
            }
        }

        /// <summary>
        /// Returns the existing open channel, or creates and initialises a new one.
        /// Thread-safe via <see cref="_channelLock"/>.
        /// </summary>
        private async Task<IChannel> GetOrCreateChannelAsync(CancellationToken cancellationToken)
        {
            if (_channel is { IsClosed: false })
                return _channel;

            await _channelLock.WaitAsync(cancellationToken);
            try
            {
                // Double-check after acquiring the lock.
                if (_channel is { IsClosed: false })
                    return _channel;

                _logger.LogDebug("Creating publisher channel and declaring exchange '{Exchange}'.", _exchangeName);

                _channel = await _connection.CreateChannelAsync();

                await _channel.ExchangeDeclareAsync(
                    exchange: _exchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: cancellationToken);

                _logger.LogDebug("Publisher channel ready, exchange '{Exchange}' declared.", _exchangeName);

                return _channel;
            }
            finally
            {
                _channelLock.Release();
            }
        }

        private async Task InvalidateChannelAsync()
        {
            await _channelLock.WaitAsync();
            try
            {
                if (_channel is not null)
                {
                    try { _channel.Dispose(); } catch (Exception ex) { _logger.LogDebug(ex, "Exception while disposing invalidated channel (best effort)."); }
                    _channel = null;
                }
            }
            finally
            {
                _channelLock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _channelLock.WaitAsync();
            try
            {
                if (_channel is not null)
                {
                    _channel.Dispose();
                    _channel = null;
                }
            }
            finally
            {
                _channelLock.Release();
            }

            _channelLock.Dispose();
        }
    }
}
