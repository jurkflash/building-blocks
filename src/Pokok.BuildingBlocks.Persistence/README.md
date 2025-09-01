# Pokok.BuildingBlocks.Persistence

## Purpose
EF Core abstractions for repository pattern, Unit of Work, and database interactions in Pokok architecture.

## Installation
Install via NuGet:
```
dotnet add package Pokok.BuildingBlocks.Persistence
```

## Example Usage
```csharp
// Example: Using IRepository
await repository.AddAsync(entity);
```

## Project Decisions
- Repository and Unit of Work patterns.
- Centralized dependency management via Directory.Packages.props.