using Pokok.BuildingBlocks.Domain.Events;

namespace Pokok.BuildingBlocks.Cqrs.Events
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    }
}
