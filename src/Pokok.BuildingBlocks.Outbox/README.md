# Pokok.BuildingBlocks.Outbox

> **Layer:** Infrastructure · **Dependencies:** Domain, Messaging, EF Core, Hosting · **Manifest:** [MODULE_MANIFEST.yaml](MODULE_MANIFEST.yaml)

## Purpose

Transactional outbox pattern for reliable, at-least-once message delivery in distributed systems. Ensures database writes and outgoing messages are eventually consistent by persisting messages in the same transaction as business data, then publishing them via a background processor.

## Installation

```sh
dotnet add package Pokok.BuildingBlocks.Outbox
```

## Public API

| Type | Name | Description |
|------|------|-------------|
| Interface | `IOutboxMessageRepository` | CRUD for outbox messages (add, query unprocessed, mark processed/failed) |
| Concrete | `OutboxMessage` | Message entity with state machine (New → Processed / Failed) |
| Concrete | `OutboxOptions` | Polling interval config (default: 10 seconds) |
| Concrete | `OutboxDbContext` | EF Core DbContext for the outbox table |
| Concrete | `OutboxMessageRepository` | Default EF Core-backed repository |
| Concrete | `OutboxProcessorHostedService<T>` | Background service polling and publishing messages |
| Static | `ServiceCollectionExtensions` | `AddOutboxProcessor<TDbContext>()` for DI registration |

## Quick Start

```csharp
// 1. Register the outbox processor
services.AddOutboxProcessor<AppDbContext>();
services.Configure<OutboxOptions>(o => o.Interval = TimeSpan.FromSeconds(5));

// 2. Write outbox messages in the same transaction as business data
public class PlaceOrderHandler : ICommandHandler<PlaceOrder, Guid>
{
    private readonly IRepository<Order> _orders;
    private readonly IOutboxMessageRepository _outbox;
    private readonly IUnitOfWork _uow;

    public async Task<Guid> HandleAsync(PlaceOrder cmd, CancellationToken ct)
    {
        var order = new Order(cmd.Product);
        await _orders.AddAsync(order, ct);

        var message = new OutboxMessage(
            OutboxMessageType.Custom("order.placed"),
            JsonSerializer.Serialize(order),
            sourceApp: "order-service");
        await _outbox.AddAsync(message, ct);

        await _uow.CompleteAsync(ct); // Both saved atomically
        return order.Id;
    }
}
```

## Message State Machine

```
┌─────┐    publish OK    ┌───────────┐
│ New │ ───────────────► │ Processed │
└──┬──┘                  └───────────┘
   │ publish fails
   ▼
┌────────┐   retry on
│ Failed │ ◄─── next poll
└────────┘   (stays eligible)
```

## Behavioral Contracts

| Contract | Enforced By |
|----------|-------------|
| At-least-once delivery | `MarkAsProcessed` only after successful publish; failed messages remain eligible |
| Polling processor | Background service polls every `OutboxOptions.Interval` for up to 10 messages |
| Atomic write with business data | Convention — add `OutboxMessage` to same DbContext/transaction |
| Failed messages are retried | `MarkAsFailed` sets `Error` but keeps `ProcessedOnUtc` null |

## Failure Modes

| Failure | Impact | Recovery |
|---------|--------|----------|
| Publish failure | Message marked as failed; retried next cycle | Fix broker issue; processor self-heals |
| Database unavailable | Processor logs error and waits | Restore connectivity; next cycle retries |
| Duplicate delivery | Message published twice (crash after publish, before marking) | Design consumers for idempotency |
| Poison message | Repeatedly fails, retried every cycle | Monitor `Error` field; implement max-retry logic |

## Rules of Engagement

1. **Add `OutboxMessage` to the same transaction** as your business entity changes.
2. **Design consumers to be idempotent** — at-least-once means possible duplicates.
3. **Register via `AddOutboxProcessor<TDbContext>()`** for repository + hosted service.
4. **Configure `OutboxOptions.Interval`** based on latency vs. database load trade-off.
5. **Monitor the `Error` field** for stuck or repeatedly failing messages.

## Project Decisions

- Polling-based processor (not change-stream or trigger-based) for simplicity and portability.
- 10-message batch size per poll cycle.
- Integrates with `IMessagePublisher` from the Messaging module for broker-agnostic publishing.