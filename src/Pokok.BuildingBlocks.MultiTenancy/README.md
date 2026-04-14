# Pokok.BuildingBlocks.MultiTenancy

> **Layer:** Domain · **Dependencies:** Domain · **Manifest:** [MODULE_MANIFEST.yaml](MODULE_MANIFEST.yaml)

## Purpose

Shared abstractions and utilities for multi-tenant applications using domain-driven design. Provides tenant context resolution and entity ownership markers.

## Installation

```sh
dotnet add package Pokok.BuildingBlocks.MultiTenancy
```

## Public API

| Type | Name | Description |
|------|------|-------------|
| Interface | `ITenantProvider` | Resolves the current tenant context; returns `null` for system-level operations |
| Interface | `ITenantScoped` | Marker for entities owned by a specific tenant (immutable `TenantId`) |

## Quick Start

```csharp
// 1. Implement ITenantProvider for your auth scheme
public class JwtTenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _accessor;
    public JwtTenantProvider(IHttpContextAccessor accessor) => _accessor = accessor;

    public TenantId? GetCurrentTenantId()
    {
        var claim = _accessor.HttpContext?.User?.FindFirst("tenant_id")?.Value;
        return claim != null ? new TenantId(claim) : null;
    }
}

// 2. Mark tenant-owned entities
public class Product : Entity<ProductId>, ITenantScoped
{
    public TenantId TenantId { get; private set; }

    public Product(ProductId id, TenantId tenantId) : base(id)
    {
        TenantId = tenantId;
    }
}

// 3. Register in DI
services.AddScoped<ITenantProvider, JwtTenantProvider>();
```

## Behavioral Contracts

| Contract | Enforced By |
|----------|-------------|
| Tenant ID is immutable | `ITenantScoped.TenantId` has no setter |
| Nullable tenant context | `ITenantProvider` returns `TenantId?` |
| TenantId rejects empty values | Constructor validation throws on null/empty (Domain module) |

## Failure Modes

| Failure | Trigger | Recovery |
|---------|---------|----------|
| Missing tenant context | `GetCurrentTenantId()` returns null where tenant is required | Validate non-null before creating tenant-scoped entities |
| Missing DI registration | `ITenantProvider` not registered | Register an implementation at startup |
| Cross-tenant data access | Queries not filtered by `TenantId` | Apply tenant filters to all `ITenantScoped` queries; consider EF Core global query filters |

## Rules of Engagement

1. **Implement `ITenantProvider`** to extract tenant context from your request pipeline (JWT claim, HTTP header, subdomain).
2. **Apply `ITenantScoped`** to all entities belonging to a specific tenant.
3. **Always filter queries** for `ITenantScoped` entities by the current tenant ID.
4. **Validate non-null tenant** before creating tenant-scoped entities.

## Project Decisions

- Abstraction-only module — tenant resolution is application-specific.
- Uses `TenantId` value object from the Domain shared kernel for type safety.
- `TenantEntity` base class is planned but currently commented out pending design confirmation.