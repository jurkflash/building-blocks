# Pokok Building Blocks

Pokok Building Blocks is a set of modular libraries for .NET 8, providing abstractions and utilities for building robust, scalable, and maintainable microservices and applications. It includes support for CQRS, persistence, domain-driven design, messaging, outbox pattern, multi-tenancy, and more.

## Features
- CQRS and domain event dispatching
- Repository and Unit of Work patterns (EF Core)
- Outbox pattern for reliable messaging
- Multi-tenancy abstractions
- Common utilities and value objects
- Messaging integrations (RabbitMQ, Email)

## Documentation

- **[Usage Guide](docs/USAGE_GUIDE.md)** - Comprehensive guide with examples for all building blocks
- **[Example Application](examples/OrderManagement/)** - Working Order Management API demonstrating the building blocks in action
- **[Contributing](docs/CONTRIBUTING.md)** - Guidelines for contributing to this project

## Quick Start

### Installation

Add the desired building blocks to your project via NuGet:

```bash
dotnet add package Pokok.BuildingBlocks.Domain
dotnet add package Pokok.BuildingBlocks.Cqrs
dotnet add package Pokok.BuildingBlocks.Persistence
# ... add other packages as needed
```

### Simple Example

Here's a minimal example using the CQRS and Domain building blocks:

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

For a complete working example, see the [Order Management Example](examples/OrderManagement/).

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

## Getting Started

1. Clone the repository
2. Open in Visual Studio 2022 or later (or use the .NET CLI)
3. Build the solution:
   ```sh
   dotnet build
   ```
4. Run the example application:
   ```sh
   cd examples/OrderManagement/OrderManagement.Api
   dotnet run
   ```
5. Navigate to `http://localhost:5000` to see the Swagger UI

## Learn More

- **[Full Usage Guide](docs/USAGE_GUIDE.md)** - Detailed documentation with examples for each building block
- **[Example Application](examples/OrderManagement/)** - Complete working example
- **[Source Code](src/)** - Browse the building block implementations

## Build

```sh
dotnet build
```

## Testing

```sh
dotnet test
```

## License
MIT

## Contributing
Pull requests and issues are welcome! See [CONTRIBUTING.md](docs/CONTRIBUTING.md) for guidelines.

## Security
See SECURITY.md for vulnerability reporting.
