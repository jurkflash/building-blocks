# Pokok.BuildingBlocks.Persistence

> **Layer:** Infrastructure · **Dependencies:** Common, Cqrs, EF Core · **Manifest:** [MODULE_MANIFEST.yaml](MODULE_MANIFEST.yaml)

## Purpose

EF Core abstractions for repository pattern, Unit of Work, specification pattern, automatic auditing, and soft delete. Provides a consistent data access layer with built-in cross-cutting concerns.

## Installation

```sh
dotnet add package Pokok.BuildingBlocks.Persistence
```

## Public API

### Entity Contracts

| Interface | Purpose | Auto-Behavior |
|-----------|---------|---------------|
| `IEntity` | Persistable entity with `Guid Id` | — |
| `IAuditable` | Audit trail fields (`CreatedAtUtc`, `CreatedBy`, etc.) | Populated on `SaveChanges` |
| `ISoftDeletable` | Soft delete fields (`IsDeleted`, `DeletedAtUtc`) | Hard deletes converted to soft deletes |
| `EntityBase` | Abstract base implementing all three | Convenience class |

### Data Access

| Type | Name | Description |
|------|------|-------------|
| Interface | `IRepository<T>` | Generic CRUD operations (Get, Find, Add, Remove, Count, Exists) |
| Interface | `IUnitOfWork` | Atomic save + domain event dispatch |
| Interface | `ISpecification<T>` | Reusable query criteria as expression objects |
| Abstract | `RepositoryBase<T>` | EF Core-backed `IRepository<T>` implementation |
| Concrete | `UnitOfWork<TContext>` | Default `IUnitOfWork` — saves then dispatches events |
| Abstract | `DbContextBase` | Auto audit trail + soft delete on `SaveChanges` |

### Specifications

| Type | Purpose |
|------|---------|
| `BaseSpecification<T>` | Criteria + optional paging |
| `PaginatedSpecification<T>` | Validated page number + page size |
| `AndSpecification<T>` | Logical AND combiner |
| `OrSpecification<T>` | Logical OR combiner |
| `NotSpecification<T>` | Logical NOT |
| `SpecificationEvaluator<T>` | Applies specifications to `IQueryable<T>` |

## Quick Start

```csharp
// 1. Create your DbContext inheriting DbContextBase
public class AppDbContext : DbContextBase
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyGlobalConfigurations(); // Soft delete filters + UTC conversion
    }
}

// 2. Create a repository
public class OrderRepository : RepositoryBase<Order, AppDbContext>
{
    public OrderRepository(AppDbContext context) : base(context) { }
}

// 3. Use in a handler
public class CreateOrderHandler : ICommandHandler<CreateOrder, Guid>
{
    private readonly IRepository<Order> _repo;
    private readonly IUnitOfWork _uow;

    public async Task<Guid> HandleAsync(CreateOrder cmd, CancellationToken ct)
    {
        var order = new Order(cmd.Product);
        await _repo.AddAsync(order, ct);
        await _uow.CompleteAsync(ct); // Saves + dispatches domain events
        return order.Id;
    }
}
```

## Behavioral Contracts

| Contract | Enforced By |
|----------|-------------|
| Changes not persisted until `CompleteAsync` | Repository uses change tracker only; UnitOfWork calls `SaveChanges` |
| Automatic audit trail for `IAuditable` | `DbContextBase.SaveChangesAsync` sets timestamps and user IDs |
| Hard deletes → soft deletes for `ISoftDeletable` | `DbContextBase` intercepts `Deleted` state and converts to `Modified` |
| Soft-deleted entities excluded from queries | Global query filter `IsDeleted == false` via `ApplyGlobalConfigurations` |
| All DateTimes stored as UTC | `UtcDateTimeConverter` applied globally |
| Domain events dispatched after persistence | UnitOfWork: SaveChanges first, then extract and dispatch events |

## Failure Modes

| Failure | Exception | Recovery |
|---------|-----------|----------|
| Missing `ICurrentUserService` | `InvalidOperationException` | Register an implementation |
| Concurrency conflict | `DbUpdateConcurrencyException` | Reload entity, reapply changes, retry |
| Invalid pagination | `ArgumentOutOfRangeException` | Use positive page number and page size |
| Event dispatch failure | Propagated from handler | Database changes already committed; design handlers for idempotency |

## Rules of Engagement

1. **Always call `IUnitOfWork.CompleteAsync()`** to persist repository changes.
2. **Inherit from `DbContextBase`** (not `DbContext`) for automatic audit and soft delete.
3. **Call `ApplyGlobalConfigurations()`** in `OnModelCreating` for soft delete filters and UTC conversion.
4. **Use `IRepository<T>`** for data access — avoid exposing `DbContext` directly.
5. **Implement `IAuditable` / `ISoftDeletable`** on entities that need these features.

## Project Decisions

- Repository and Unit of Work patterns for testable data access.
- Automatic audit trail via `ICurrentUserService` integration.
- Specification pattern for reusable, composable queries.
- Centralized dependency management via `Directory.Packages.props`.