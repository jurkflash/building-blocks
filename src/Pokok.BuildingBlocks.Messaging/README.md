# Pokok.BuildingBlocks.Messaging

> **Layer:** Infrastructure · **Dependencies:** RabbitMQ.Client, Hosting, Logging, Options · **Manifest:** [MODULE_MANIFEST.yaml](MODULE_MANIFEST.yaml)

## Purpose

Messaging abstractions and RabbitMQ integration for publish/subscribe patterns in distributed systems. Provides a transport-agnostic `IMessagePublisher` interface with a concrete RabbitMQ implementation using topic exchanges.

## Installation

```sh
dotnet add package Pokok.BuildingBlocks.Messaging
```

## Public API

| Type | Name | Description |
|------|------|-------------|
| Interface | `IMessagePublisher` | Transport-agnostic message publishing |
| Interface | `IRabbitMQConnection` | RabbitMQ connection lifecycle management |
| Interface | `IRabbitMQMessageHandler<T>` | Typed message handler for consumers |
| Concrete | `RabbitMQOptions` | Connection config (HostName, Port, UserName, Password) |
| Concrete | `RabbitMQConnection` | Single shared connection, creates channels on demand |
| Concrete | `RabbitMQMessagePublisher` | Topic exchange publisher with persistent, mandatory messages |
| Concrete | `RabbitMQMessageConsumer<T>` | BackgroundService consumer with JSON deserialization |

## Quick Start

```csharp
// 1. Configure RabbitMQ
services.Configure<RabbitMQOptions>(config.GetSection("RabbitMQ"));
services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
services.AddScoped<IMessagePublisher, RabbitMQMessagePublisher>();

// 2. Publish a message
await publisher.PublishAsync("order.created", JsonSerializer.Serialize(order), ct);

// 3. Consume messages
public class OrderHandler : IRabbitMQMessageHandler<OrderCreatedEvent>
{
    public Task HandleAsync(OrderCreatedEvent message, CancellationToken ct)
    {
        // Process the message
        return Task.CompletedTask;
    }
}
```

## Behavioral Contracts

| Contract | Enforced By |
|----------|-------------|
| Single shared TCP connection | Lazy init in `RabbitMQConnection.CreateChannelAsync` |
| Persistent messages | `DeliveryMode.Persistent` on all published messages |
| Mandatory routing | `mandatory: true` — unroutable messages cause exceptions |
| Topic exchange (`pokok.exchange`) | Exchange declared as Topic in publisher and consumer |
| JSON serialization | `ContentType: application/json`; consumer deserializes with `System.Text.Json` |
| Messages ACKed after handling | Consumer ACKs regardless of handler success/failure |

## Failure Modes

| Failure | Exception | Recovery |
|---------|-----------|----------|
| Broker unreachable | `BrokerUnreachableException` | Verify server, host, port, credentials |
| Unroutable message | `PublishException` | Ensure consumer with matching routing key is running |
| Deserialization failure | `JsonException` (logged, not thrown) | Align publisher/consumer schemas |
| Handler exception | Logged at ERROR; message still ACKed | Fix handler bug; use Outbox for retries |
| Connection dropped | Various RabbitMQ exceptions | Connection re-established lazily on next channel request |

## Rules of Engagement

1. **Configure `RabbitMQOptions`** via `IOptions<T>` in your app settings.
2. **Register `IRabbitMQConnection` as singleton** — one TCP connection per app.
3. **Use `IMessagePublisher`** (not the concrete class) for testability.
4. **Ensure consumers are running** before publishing, or accept unroutable message failures.
5. **Implement `IRabbitMQMessageHandler<T>`** for each consumed message type.

## Project Decisions

- Transport abstraction via `IMessagePublisher` enables swapping brokers.
- RabbitMQ topic exchanges for flexible routing.
- Persistent + mandatory messages for reliability.
- Consumer runs as a `BackgroundService` for hosted app integration.