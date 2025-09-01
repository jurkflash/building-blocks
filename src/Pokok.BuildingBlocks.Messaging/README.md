# Pokok.BuildingBlocks.Messaging

## Purpose
Messaging integrations for Pokok Platform (RabbitMQ, etc.).

## Installation
Install via NuGet:
```
dotnet add package Pokok.BuildingBlocks.Messaging
```

## Example Usage
```csharp
// Example: Publishing a message
await messagePublisher.PublishAsync(message);
```

## Project Decisions
- RabbitMQ integration and abstractions.