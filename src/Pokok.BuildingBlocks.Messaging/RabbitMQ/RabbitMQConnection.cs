using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokok.BuildingBlocks.Messaging.RabbitMQ
{
    public class RabbitMQConnection : IRabbitMQConnection, IAsyncDisposable
    {
        private IConnection? _connection;

        private readonly IConnectionFactory _connectionFactory;

        public RabbitMQConnection(IOptions<RabbitMQOptions> options)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = options.Value.HostName,
                Port = options.Value.Port,
                UserName = options.Value.UserName,
                Password = options.Value.Password,
            };
        }

        public async Task<IChannel> CreateChannelAsync()
        {
            if (_connection is null || !_connection.IsOpen)
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
            }

            return await _connection.CreateChannelAsync(); // This is still valid
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection is not null)
            {
                await _connection.CloseAsync();
                _connection.Dispose();
            }
        }
    }
}
