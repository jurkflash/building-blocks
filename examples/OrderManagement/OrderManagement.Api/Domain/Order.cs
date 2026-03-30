using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Events;
using Pokok.BuildingBlocks.Domain.Exceptions;

namespace OrderManagement.Api.Domain;

/// <summary>
/// Order aggregate root demonstrating DDD patterns
/// </summary>
public class Order : AggregateRoot<Guid>
{
    public string CustomerName { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? ModifiedAtUtc { get; private set; }

    // EF Core requires a parameterless constructor
    private Order() : base(Guid.NewGuid())
    {
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Order Create(string customerName, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new DomainException("Customer name is required");

        if (amount <= 0)
            throw new DomainException("Amount must be greater than zero");

        var order = new Order
        {
            CustomerName = customerName,
            Amount = amount,
            CreatedAtUtc = DateTime.UtcNow
        };

        // Raise domain event
        order.AddDomainEvent(new OrderCreatedEvent(order.Id, order.CustomerName, order.Amount));

        return order;
    }

    public void UpdateCustomerName(string newCustomerName)
    {
        if (string.IsNullOrWhiteSpace(newCustomerName))
            throw new DomainException("Customer name is required");

        CustomerName = newCustomerName;
        ModifiedAtUtc = DateTime.UtcNow;
    }
}

/// <summary>
/// Domain event raised when an order is created
/// </summary>
public class OrderCreatedEvent : DomainEventBase
{
    public Guid OrderId { get; }
    public string CustomerName { get; }
    public decimal Amount { get; }

    public OrderCreatedEvent(Guid orderId, string customerName, decimal amount)
    {
        OrderId = orderId;
        CustomerName = customerName;
        Amount = amount;
    }
}
