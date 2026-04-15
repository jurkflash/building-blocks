# Pokok.BuildingBlocks.Common

> **Layer:** Shared Kernel · **Dependencies:** None

## Purpose

Common utility classes, extensions, and primitives shared across all Pokok microservices and building block modules. This is the **shared kernel** — intentionally lightweight and dependency-free so every other module can reference it safely.

## Installation

```sh
dotnet add package Pokok.BuildingBlocks.Common
```

## Public API

| Type | Name | Description |
|------|------|-------------|
| Interface | `ICurrentUserService` | Provides the authenticated user's identity (`UserId`). Returns `null` when unauthenticated. |

## Quick Start

```csharp
// 1. Implement the interface in your application layer
public class HttpCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;
    public HttpCurrentUserService(IHttpContextAccessor accessor) => _accessor = accessor;
    public string? UserId => _accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}

// 2. Register in DI
services.AddScoped<ICurrentUserService, HttpCurrentUserService>();
```

## Behavioral Contracts

| Contract | Description |
|----------|-------------|
| **Nullable identity** | `UserId` may return `null`. All consumers must handle null gracefully (e.g., default to `"system"`). |
| **Zero dependencies** | This module must never add NuGet package references to remain a safe shared kernel. |

## Failure Modes

| Failure | Trigger | Recovery |
|---------|---------|----------|
| Missing DI registration | `ICurrentUserService` not registered | Register an implementation during application startup |

## Rules of Engagement

1. **Implement `ICurrentUserService`** in your application layer and register it in DI — the persistence module depends on it for audit trails.
2. **Do not add domain or infrastructure concerns** to this module — it must remain lightweight and free of business logic.

## Extension Points

- **`ICurrentUserService`** — Implement to provide user identity from your authentication middleware (JWT, session, HTTP context).

## Project Decisions

- Shared kernel pattern — minimal surface, no external dependencies.
- Audit trail integration happens via the Persistence module's `DbContextBase`.