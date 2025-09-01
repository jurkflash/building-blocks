# Pokok.BuildingBlocks.Cqrs

## Purpose
CQRS and domain event dispatching for Pokok Platform.

## Installation
Install via NuGet:
```
dotnet add package Pokok.BuildingBlocks.Cqrs
```

## Example Usage
```csharp
// Example: Dispatching a domain event
await dispatcher.DispatchAsync(event);
```

## Project Decisions
- CQRS pattern and event dispatcher implementation.