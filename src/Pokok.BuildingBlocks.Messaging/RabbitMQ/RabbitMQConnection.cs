using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Pokok.BuildingBlocks.Messaging.RabbitMQ
{
    /// <summary>
    /// Default <see cref="IRabbitMQConnection"/> implementation that maintains a single shared TCP connection
    /// to the RabbitMQ broker and creates new channels on demand. The connection is lazily initialized
    /// on the first call to <see cref="CreateChannelAsync"/>.
    /// </summary>
    public class RabbitMQConnection : IRabbitMQConnection, IAsyncDisposable
    {
        private IConnection? _connection;
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMQConnection> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="RabbitMQConnection"/> with the specified options and logger.
        /// </summary>
        /// <param name="options">RabbitMQ connection configuration options.</param>
        /// <param name="logger">Logger for connection lifecycle events.</param>
        public RabbitMQConnection(IOptions<RabbitMQOptions> options, ILogger<RabbitMQConnection> logger)
        {
            _logger = logger;
            _connectionFactory = new ConnectionFactory
            {
                HostName = options.Value.HostName,
                Port = options.Value.Port,
                UserName = options.Value.UserName,
                Password = options.Value.Password,
            };
        }

        /// <inheritdoc />
        public async Task<IChannel> CreateChannelAsync()
        {
            if (_connection is null || !_connection.IsOpen)
            {
                _logger.LogInformation("Creating new RabbitMQ connection");
                _connection = await _connectionFactory.CreateConnectionAsync();
                _logger.LogInformation("Connected to RabbitMQ server at {Host}:{Port}", _connection.Endpoint.HostName, _connection.Endpoint.Port);
            }

            _logger.LogInformation("Creating new RabbitMQ channel.");
            return await _connection.CreateChannelAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection is not null)
            {
                _logger.LogInformation("Closing RabbitMQ connection.");
                await _connection.CloseAsync();
                _connection.Dispose();
            }
        }
    }
}
