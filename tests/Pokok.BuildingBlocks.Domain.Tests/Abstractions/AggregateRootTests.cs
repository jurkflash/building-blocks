using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Events;
using Xunit;

namespace Pokok.BuildingBlocks.Domain.Abstractions;

internal sealed class OrderCreatedEvent : DomainEventBase { }

internal sealed class OrderAggregate : AggregateRoot<Guid>
{
    public OrderAggregate(Guid id) : base(id) { }

    public void Create() => AddDomainEvent(new OrderCreatedEvent());
}

public class AggregateRootTests
{
    [Fact]
    public void AddDomainEvent_WhenEventAdded_DomainEventsContainsEvent()
    {
        var aggregate = new OrderAggregate(Guid.NewGuid());

        aggregate.Create();

        Assert.Single(aggregate.DomainEvents);
        Assert.IsType<OrderCreatedEvent>(aggregate.DomainEvents.First());
    }

    [Fact]
    public void ClearDomainEvents_AfterEventsAdded_DomainEventsIsEmpty()
    {
        var aggregate = new OrderAggregate(Guid.NewGuid());
        aggregate.Create();
        aggregate.Create();

        aggregate.ClearDomainEvents();

        Assert.Empty(aggregate.DomainEvents);
    }

    [Fact]
    public void DomainEvents_WhenNoEventsAdded_IsEmpty()
    {
        var aggregate = new OrderAggregate(Guid.NewGuid());

        Assert.Empty(aggregate.DomainEvents);
    }

    [Fact]
    public void AddDomainEvent_MultipleEventsAdded_AllEventsAreRetained()
    {
        var aggregate = new OrderAggregate(Guid.NewGuid());

        aggregate.Create();
        aggregate.Create();

        Assert.Equal(2, aggregate.DomainEvents.Count);
    }
}
