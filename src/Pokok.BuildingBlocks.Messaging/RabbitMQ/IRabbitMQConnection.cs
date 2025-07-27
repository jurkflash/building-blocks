using RabbitMQ.Client;

namespace Pokok.BuildingBlocks.Messaging.RabbitMQ
{
    public interface IRabbitMQConnection : IAsyncDisposable
    {
        Task<IChannel> CreateChannelAsync();
    }
}
