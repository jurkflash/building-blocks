# Pokok.BuildingBlocks.Domain

> **Layer:** Domain · **Dependencies:** None

## Purpose

Core DDD building blocks — AggregateRoot, Entity, ValueObject, domain events, and a library of shared-kernel value objects (Email, Money, Address, etc.). This module is **dependency-free** to keep the domain layer pure and portable.

## Installation

```sh
dotnet add package Pokok.BuildingBlocks.Domain
```

## Public API

### Core Abstractions

| Type | Name | Description |
|------|------|-------------|
| Abstract | `ValueObject` | Structural equality via `GetEqualityComponents()`. Immutable after construction. |
| Abstract | `SingleValueObject<T>` | Wraps a single value with validation. Throws `ArgumentNullException` on null. |
| Abstract | `EntityId<T>` | Strongly-typed entity identifier extending `SingleValueObject`. |
| Abstract | `Entity<TId>` | Identity-based equality. Mutable state, equality by `Id` only. |
| Abstract | `AggregateRoot<TId>` | Root entity with domain event collection (`AddDomainEvent`, `ClearDomainEvents`). |
| Interface | `IAggregateRoot` | Contract for aggregate roots that collect domain events. |
| Interface | `IDomainEvent` | Represents a domain fact with `OccurredOn` timestamp. |
| Interface | `IDomainEventHandler<T>` | Handles a specific domain event type. |
| Abstract | `DomainEventBase` | Convenience base — sets `OccurredOn` to `DateTime.UtcNow`. |
| Exception | `DomainException` | Thrown when a domain invariant or business rule is violated. |

### Shared Kernel Value Objects

| Name | Validation | Throws `DomainException` when |
|------|------------|-------------------------------|
| `Email` | Regex `^[^@\s]+@[^@\s]+\.[^@\s]+$` | Invalid format |
| `PhoneNumber` | Regex `^\+?[0-9\s\-()]{7,20}$` | Invalid format |
| `PersonName` | FirstName + LastName required | Either name empty |
| `Money` | Amount ≥ 0, Currency required | Negative amount or currency mismatch on addition |
| `Address` | All fields required | Any field empty |
| `UserId` | Non-empty string | Null or empty |
| `DisplayName` | Non-empty string | Null or empty |
| `TenantId` | Non-empty string | Null or empty |
| `Url` | Absolute URI | Not a valid absolute URL |
| `DateTimeRange` | Start < End | Start ≥ End |
| `Versioned<T>` | Value + version number | Null value |
| `OutboxMessageType` | Non-empty string | Null or empty |

## Quick Start

```csharp
// Define a value object
public sealed class OrderId : EntityId<Guid>
{
    public OrderId(Guid value) : base(value) { }
}

// Define an aggregate root
public class Order : AggregateRoot<OrderId>
{
    public Money Total { get; private set; }

    public void Place()
    {
        AddDomainEvent(new OrderPlacedEvent(Id));
    }
}

// Define a domain event
public class OrderPlacedEvent : DomainEventBase
{
    public OrderId OrderId { get; }
    public OrderPlacedEvent(OrderId orderId) => OrderId = orderId;
}
```

## Behavioral Contracts

| Contract | Enforced By |
|----------|-------------|
| Value objects use structural equality | `GetEqualityComponents()` comparison in `Equals()` and `==` |
| Entities use identity equality | `Entity<TId>.Equals()` compares `Id` only |
| Value objects are immutable | No public setters; validation in constructor |
| Aggregate events follow add → dispatch → clear lifecycle | `AddDomainEvent` (protected), `ClearDomainEvents` (public) |
| Construction-time validation | Shared kernel value objects throw `DomainException` in constructor |

## Failure Modes

All shared kernel value objects throw `DomainException` on invalid input during construction. See the validation table above for specific triggers.

## Rules of Engagement

1. **Inherit `ValueObject`** for types defined by attributes, not identity.
2. **Inherit `Entity<TId>`** for types with persistent identity.
3. **Inherit `AggregateRoot<TId>`** for consistency boundary roots only.
4. **Raise events via `AddDomainEvent()`** inside aggregate methods — never externally.
5. **Throw `DomainException`** for business rule violations — not generic exceptions.
6. **Never add infrastructure dependencies** (EF Core, HTTP, messaging) to this module.

## Project Decisions

- Pure domain layer — zero external dependencies.
- Shared kernel value objects provide validated building blocks for common concepts.
- DDD tactical patterns (Aggregate, Entity, Value Object, Domain Event) as base classes.