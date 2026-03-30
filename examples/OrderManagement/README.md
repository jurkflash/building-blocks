# Order Management Example

This example demonstrates how to use the Pokok Building Blocks to build a simple Order Management API.

## Features Demonstrated

This example showcases the following building blocks:

1. **Common** - `ICurrentUserService` for user context
2. **Domain** - Domain entities, aggregate roots, and domain events
3. **CQRS** - Command/Query separation with handlers and validation
4. **Persistence** - Repository pattern, Unit of Work, and EF Core integration

## Architecture

The application follows Clean Architecture principles:

```
Application/
  ├── Commands (CreateOrderCommand, CreateOrderCommandHandler, Validator)
  ├── Queries (GetOrderQuery, GetAllOrdersQuery, Handlers)
  └── EventHandlers (OrderCreatedEventHandler)

Domain/
  └── Order (Aggregate Root with domain events)

Infrastructure/
  ├── OrderDbContext (EF Core DbContext)
  └── SimpleCurrentUserService (User context implementation)
```

## Running the Example

### Prerequisites
- .NET 8.0 SDK or later

### Steps

1. Navigate to the example directory:
```bash
cd examples/OrderManagement/OrderManagement.Api
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Run the application:
```bash
dotnet run
```

4. Open your browser to `http://localhost:5000` (redirects to Swagger UI)

## API Endpoints

### Create Order
```http
POST /api/orders
Content-Type: application/json

{
  "customerName": "John Doe",
  "amount": 99.99
}
```

Response:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Get All Orders
```http
GET /api/orders
```

Response:
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "customerName": "John Doe",
    "amount": 99.99,
    "createdAtUtc": "2024-03-30T12:00:00Z",
    "modifiedAtUtc": null
  }
]
```

### Get Order by ID
```http
GET /api/orders/{id}
```

Response:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "customerName": "John Doe",
  "amount": 99.99,
  "createdAtUtc": "2024-03-30T12:00:00Z",
  "modifiedAtUtc": null
}
```

## Testing the Example

### Using cURL

Create an order:
```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{"customerName":"John Doe","amount":99.99}'
```

Get all orders:
```bash
curl http://localhost:5000/api/orders
```

Get specific order:
```bash
curl http://localhost:5000/api/orders/{order-id}
```

### Using Swagger UI

1. Run the application
2. Navigate to `http://localhost:5000/swagger`
3. Use the interactive UI to test the endpoints

## Validation Examples

The example includes validation that will reject invalid requests:

**Missing customer name:**
```json
{
  "customerName": "",
  "amount": 99.99
}
```
Returns: `400 Bad Request` with error `"Customer name is required"`

**Invalid amount:**
```json
{
  "customerName": "John Doe",
  "amount": -10
}
```
Returns: `400 Bad Request` with error `"Amount must be greater than zero"`

**Amount too large:**
```json
{
  "customerName": "John Doe",
  "amount": 2000000
}
```
Returns: `400 Bad Request` with error `"Amount cannot exceed 1,000,000"`

## Key Concepts Illustrated

### 1. Domain-Driven Design
The `Order` class is an aggregate root that:
- Encapsulates business logic
- Raises domain events
- Enforces invariants
- Has a private constructor to ensure creation only through factory method

### 2. CQRS Pattern
Commands and queries are separated:
- **Commands** change state (`CreateOrderCommand`)
- **Queries** read state (`GetOrderQuery`, `GetAllOrdersQuery`)
- Each has its own handler
- Validation is applied to commands

### 3. Domain Events
When an order is created:
1. The `Order.Create()` method raises an `OrderCreatedEvent`
2. The event is stored in the aggregate
3. When `UnitOfWork.CompleteAsync()` is called, events are dispatched
4. `OrderCreatedEventHandler` processes the event
5. Events are then cleared from the aggregate

### 4. Unit of Work Pattern
The `IUnitOfWork` coordinates:
- Saving changes to the database
- Dispatching domain events
- Transaction management

### 5. Repository Pattern
The `IRepository<Order>` provides:
- Abstraction over data access
- Standard CRUD operations
- Query capabilities

## Database

This example uses an in-memory database for simplicity. In a production application, you would:

1. Replace `UseInMemoryDatabase` with your database provider (e.g., `UseSqlServer`)
2. Add connection strings to `appsettings.json`
3. Create and apply migrations

Example for SQL Server:
```csharp
builder.Services.AddDbContext<OrderDbContext>((provider, options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
```

## Next Steps

To extend this example with more building blocks:

1. **Add Messaging** - Publish order events to RabbitMQ
2. **Add Outbox** - Use transactional outbox for reliable messaging
3. **Add Multi-Tenancy** - Implement tenant isolation
4. **Add Email** - Send order confirmation emails

See the [Usage Guide](../../../docs/USAGE_GUIDE.md) for detailed instructions on using all building blocks.

## Learn More

- [Usage Guide](../../../docs/USAGE_GUIDE.md) - Comprehensive guide for all building blocks
- [Main README](../../../README.md) - Project overview
- [Source Code](../../../src/) - Building block implementations
