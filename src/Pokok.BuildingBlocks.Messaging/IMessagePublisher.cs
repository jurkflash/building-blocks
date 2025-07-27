using Pokok.BuildingBlocks.Domain.SharedKernel.Enums;

namespace Pokok.BuildingBlocks.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync(OutboxMessageType messageType, string payload, CancellationToken cancellationToken);
    }
}
