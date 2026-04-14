# Pokok Building Blocks

Pokok Building Blocks is a set of modular libraries for .NET 8, providing abstractions and utilities for building robust, scalable, and maintainable microservices and applications. It includes support for CQRS, persistence, domain-driven design, messaging, outbox pattern, multi-tenancy, and more.

## Self-Describing Modules

Every module in this repository is **self-describing**. Each module contains:

- **`MODULE_MANIFEST.yaml`** — A structured manifest with the module's purpose, public API, behavioral contracts, failure modes, rules of engagement, and extension points. Machine-readable (YAML) and human-friendly.
- **`README.md`** — Enriched documentation with quick-start examples, API tables, contract summaries, and failure mode reference.

This makes the codebase readable by both humans and AI systems without needing to parse every source file. See [`docs/manifest-schema.md`](docs/manifest-schema.md) for the manifest schema reference.

## Module Catalog

| Module | Layer | Purpose |
|--------|-------|---------|
| [`Common`](src/Pokok.BuildingBlocks.Common) | Shared Kernel | User identity abstraction (`ICurrentUserService`) |
| [`Domain`](src/Pokok.BuildingBlocks.Domain) | Domain | DDD building blocks — AggregateRoot, Entity, ValueObject, domain events, shared kernel value objects |
| [`Cqrs`](src/Pokok.BuildingBlocks.Cqrs) | Application | Command/query dispatching, domain event dispatching, validation pipeline |
| [`Persistence`](src/Pokok.BuildingBlocks.Persistence) | Infrastructure | Repository, Unit of Work, specifications, automatic audit trail, soft delete |
| [`Messaging`](src/Pokok.BuildingBlocks.Messaging) | Infrastructure | RabbitMQ publish/subscribe with transport abstraction |
| [`MultiTenancy`](src/Pokok.BuildingBlocks.MultiTenancy) | Domain | Tenant context resolution and entity ownership |
| [`Outbox`](src/Pokok.BuildingBlocks.Outbox) | Infrastructure | Transactional outbox for at-least-once message delivery |
| [`Email`](src/Pokok.Messaging.Email) | Infrastructure | Email template rendering with placeholder substitution |

## Architecture

```
┌──────────────────────────────────────────────────────┐
│                   Application Layer                   │
│                    ┌──────────┐                       │
│                    │   Cqrs   │                       │
│                    └────┬─────┘                       │
├─────────────────────────┼────────────────────────────┤
│                   Domain Layer                        │
│  ┌──────────┐    ┌──────┴─────┐    ┌──────────────┐  │
│  │  Common   │   │   Domain   │    │ MultiTenancy │  │
│  └──────────┘    └────────────┘    └──────────────┘  │
├──────────────────────────────────────────────────────┤
│               Infrastructure Layer                    │
│  ┌─────────────┐  ┌───────────┐  ┌────────┐         │
│  │ Persistence │  │ Messaging │  │ Outbox │         │
│  └─────────────┘  └───────────┘  └────────┘         │
│  ┌───────┐                                           │
│  │ Email │                                           │
│  └───────┘                                           │
└──────────────────────────────────────────────────────┘
```

## Features
- CQRS and domain event dispatching
- Repository and Unit of Work patterns (EF Core)
- Outbox pattern for reliable messaging
- Multi-tenancy abstractions
- Common utilities and value objects
- Messaging integrations (RabbitMQ, Email)

## Getting Started
1. Clone the repository
2. Open in Visual Studio 2022 or later
3. Build the solution

## Usage
Reference the desired building block projects in your .NET solution. Each module's README contains quick-start examples, behavioral contracts, and rules of engagement.

## Build
```sh
dotnet build
```

## License
MIT

## Contributing
Pull requests and issues are welcome! See CONTRIBUTING.md for guidelines.

## Security
See SECURITY.md for vulnerability reporting.
