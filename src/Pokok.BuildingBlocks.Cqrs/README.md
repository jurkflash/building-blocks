# Pokok.BuildingBlocks.Cqrs

> **Layer:** Application · **Dependencies:** Domain, DI, Logging · **Manifest:** [MODULE_MANIFEST.yaml](MODULE_MANIFEST.yaml)

## Purpose

CQRS pattern implementation with command/query dispatching, domain event dispatching, and optional validation decorators. Separates read and write operations with type-safe handler resolution via dependency injection.

## Installation

```sh
dotnet add package Pokok.BuildingBlocks.Cqrs
```

## Public API

| Type | Name | Description |
|------|------|-------------|
| Interface | `ICommand<TResponse>` | Marker for command messages |
| Interface | `ICommandHandler<TCommand, TResponse>` | Handles a command and returns a result |
| Interface | `IQuery<TResponse>` | Marker for query messages |
| Interface | `IQueryHandler<TQuery, TResponse>` | Handles a query and returns a result |
| Interface | `ICommandDispatcher` | Dispatches commands to registered handlers |
| Interface | `IQueryDispatcher` | Dispatches queries to registered handlers |
| Interface | `IDomainEventDispatcher` | Dispatches domain events to all registered handlers |
| Interface | `IValidator<T>` | Validates a command/query before execution |
| Exception | `ValidationException` | Thrown with accumulated error list when validation fails |
| Static | `CqrsRegistrationExtensions` | DI registration helpers for handlers and dispatchers |

## Quick Start

```csharp
// 1. Define a command
public record CreateOrder(string Product, int Quantity) : ICommand<Guid>;

// 2. Implement the handler
public class CreateOrderHandler : ICommandHandler<CreateOrder, Guid>
{
    public Task<Guid> HandleAsync(CreateOrder command, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        // ... create order ...
        return Task.FromResult(id);
    }
}

// 3. Optionally add a validator
public class CreateOrderValidator : IValidator<CreateOrder>
{
    public void Validate(CreateOrder request)
    {
        if (request.Quantity <= 0)
            throw new ValidationException(new[] { "Quantity must be positive" });
    }
}

// 4. Register in DI
services.AddCommandHandler<CreateOrder, Guid, CreateOrderHandler, CreateOrderValidator>();
services.AddDomainEventDispatcher();

// 5. Dispatch
var orderId = await commandDispatcher.DispatchAsync<CreateOrder, Guid>(command, ct);
```

## Behavioral Contracts

| Contract | Enforced By |
|----------|-------------|
| One handler per command/query type | DI resolves a single handler; throws if missing |
| Multiple handlers per domain event | Reflection-based discovery invokes all registered handlers |
| Validation runs before handling | Decorator pattern wraps real handler; all validators run and errors accumulate |
| Queries must be side-effect free | Convention only (no runtime enforcement) |

## Failure Modes

| Failure | Exception | Recovery |
|---------|-----------|----------|
| No handler registered | `InvalidOperationException` | Register handler via `AddCommandHandler` / `AddQueryHandler` |
| Validation failure | `ValidationException` (with `Errors` list) | Fix input based on error messages and retry |
| Domain event handler throws | Propagated from handler | Fix handler bug; design handlers for idempotency |
| Reflection failure in event dispatch | `TargetInvocationException` | Ensure handler implements `IDomainEventHandler<T>` correctly |

## Rules of Engagement

1. **Define commands/queries** as records or classes implementing `ICommand<T>` / `IQuery<T>`.
2. **Register handlers via `CqrsRegistrationExtensions`** — not manually in DI.
3. **Keep handlers thin** — delegate to domain services and repositories.
4. **Register `IDomainEventDispatcher`** as singleton via `AddDomainEventDispatcher()`.

## Project Decisions

- DI-based handler resolution — no reflection for command/query dispatch.
- Reflection used only for domain event handler discovery (multiple handlers per event).
- Validation is opt-in via the decorator pattern.