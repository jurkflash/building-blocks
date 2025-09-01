# Pokok.BuildingBlocks.Domain

## Purpose
Provides building blocks for domain-driven design (DDD) in Pokok Platform.

## Installation
Install via NuGet:
```
dotnet add package Pokok.BuildingBlocks.Domain
```

## Example Usage
```csharp
// Example: Using a ValueObject
var versioned = new Versioned<string>("v1.0", 1);
```

## Project Decisions
- Implements DDD patterns: AggregateRoot, ValueObject, Specification.