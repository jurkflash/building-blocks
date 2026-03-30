---
layout: default
title: Pokok Building Blocks
---

# Pokok Building Blocks

Pokok Building Blocks is a set of modular libraries for .NET 8, providing abstractions and utilities for building robust, scalable, and maintainable microservices and applications.

## Documentation

- **[Usage Guide](USAGE_GUIDE.md)** - Comprehensive guide with examples for all building blocks
- **[Example Application](../examples/OrderManagement/)** - Working Order Management API demonstrating the building blocks in action

## Quick Links

- [GitHub Repository](https://github.com/jurkflash/building-blocks)
- [Contributing](CONTRIBUTING.md)
- [Coding Standards](coding-standards.md)
- [Dependency Management](dependency-management.md)
- [Release Process](release-process.md)

## Features

- CQRS and domain event dispatching
- Repository and Unit of Work patterns (EF Core)
- Outbox pattern for reliable messaging
- Multi-tenancy abstractions
- Common utilities and value objects
- Messaging integrations (RabbitMQ, Email)

## Getting Started

### Installation

Add the desired building blocks to your project via NuGet:

```bash
dotnet add package Pokok.BuildingBlocks.Domain
dotnet add package Pokok.BuildingBlocks.Cqrs
dotnet add package Pokok.BuildingBlocks.Persistence
# ... add other packages as needed
```

### Quick Example

```csharp
// 1. Define a domain entity
public class Order : AggregateRoot<Guid>
{
    public string CustomerName { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }

    private Order() : base(Guid.NewGuid()) { }

    public static Order Create(string customerName, decimal amount)
    {
        var order = new Order { CustomerName = customerName, Amount = amount };
        order.AddDomainEvent(new OrderCreatedEvent(order.Id));
        return order;
    }
}

// 2. Create a command
public record CreateOrderCommand(string CustomerName, decimal Amount) : ICommand<Guid>;

public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly IRepository<Order> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Guid> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = Order.Create(command.CustomerName, command.Amount);
        await _repository.AddAsync(order);
        await _unitOfWork.CompleteAsync(cancellationToken);
        return order.Id;
    }
}

// 3. Register and use
services.AddCommandHandler<CreateOrderCommand, Guid, CreateOrderHandler>();

var orderId = await commandDispatcher.DispatchAsync<CreateOrderCommand, Guid>(
    new CreateOrderCommand("John Doe", 99.99m), cancellationToken);
```

## Building Blocks

| Package | Description |
|---------|-------------|
| **Pokok.BuildingBlocks.Common** | Shared abstractions (`ICurrentUserService`) |
| **Pokok.BuildingBlocks.Domain** | DDD abstractions (Entities, Aggregates, Value Objects, Domain Events) |
| **Pokok.BuildingBlocks.Cqrs** | Command/Query pattern with handlers, validators, and dispatchers |
| **Pokok.BuildingBlocks.Persistence** | Repository pattern, Unit of Work, EF Core abstractions |
| **Pokok.BuildingBlocks.Messaging** | RabbitMQ message publishing and consuming |
| **Pokok.BuildingBlocks.Outbox** | Transactional outbox pattern for reliable messaging |
| **Pokok.BuildingBlocks.MultiTenancy** | Multi-tenant application abstractions |
| **Pokok.Messaging.Email** | Email message handling and template rendering |

## Learn More

For detailed documentation and examples, see the [Usage Guide](USAGE_GUIDE.md).

## License

MIT
