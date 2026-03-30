using OrderManagement.Api.Domain;
using Pokok.BuildingBlocks.Domain.Events;

namespace OrderManagement.Api.Application;

/// <summary>
/// Domain event handler for OrderCreatedEvent
/// Demonstrates how domain events can trigger side effects
/// </summary>
public class OrderCreatedEventHandler : IDomainEventHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Order created event handled - OrderId: {OrderId}, Customer: {CustomerName}, Amount: {Amount}",
            domainEvent.OrderId,
            domainEvent.CustomerName,
            domainEvent.Amount);

        // In a real application, you might:
        // - Send a notification email
        // - Update analytics
        // - Trigger a workflow
        // - Add message to outbox for publishing to message bus

        return Task.CompletedTask;
    }
}
