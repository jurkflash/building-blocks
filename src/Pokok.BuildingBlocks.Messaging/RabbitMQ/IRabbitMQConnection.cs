using RabbitMQ.Client;

namespace Pokok.BuildingBlocks.Messaging.RabbitMQ
{
    /// <summary>
    /// Manages RabbitMQ connection lifecycle and channel creation.
    /// Implementations should maintain a single shared connection and create channels on demand.
    /// Register as singleton to avoid unnecessary TCP connections.
    /// </summary>
    public interface IRabbitMQConnection : IAsyncDisposable
    {
        Task<IChannel> CreateChannelAsync();
    }
}
