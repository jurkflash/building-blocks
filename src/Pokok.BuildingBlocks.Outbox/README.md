# Pokok.BuildingBlocks.Outbox

## Purpose
Provides outbox pattern implementation for reliable message delivery in distributed systems.

## Installation
Install via NuGet:
```
dotnet add package Pokok.BuildingBlocks.Outbox
```

## Example Usage
```csharp
// Example: Publishing an outbox event
await outboxPublisher.PublishAsync(event);
```

## Project Decisions
- Implements outbox pattern for RabbitMQ and distributed messaging.