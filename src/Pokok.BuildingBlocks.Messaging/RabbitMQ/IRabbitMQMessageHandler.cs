namespace Pokok.BuildingBlocks.Messaging.RabbitMQ
{
    public interface IRabbitMQMessageHandler<T>
    {
        Task HandleAsync(T message, CancellationToken cancellationToken = default);
    }
}
