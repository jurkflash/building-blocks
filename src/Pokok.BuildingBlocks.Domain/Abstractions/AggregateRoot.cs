using Pokok.BuildingBlocks.Domain.Events;

namespace Pokok.BuildingBlocks.Domain.Abstractions
{
    /// <summary>
    /// Base class for aggregate root entities in domain-driven design.
    /// Collects domain events via <see cref="AddDomainEvent"/> that are dispatched by the Unit of Work after persistence.
    /// Serves as the transactional consistency boundary — only aggregate roots should be directly persisted and loaded.
    /// </summary>
    /// <typeparam name="TId">The type of the aggregate root's identifier.</typeparam>
    public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected AggregateRoot() : base() { }

        protected AggregateRoot(TId id) : base(id) { }

        protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
