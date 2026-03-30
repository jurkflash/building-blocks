# Pokok Building Blocks - Usage Guide

This guide demonstrates how to use each building block in the Pokok Building Blocks library to build robust, scalable .NET applications.

## Table of Contents

- [Installation](#installation)
- [Building Blocks Overview](#building-blocks-overview)
- [Quick Start](#quick-start)
- [Detailed Usage by Building Block](#detailed-usage-by-building-block)
  - [Common](#1-common)
  - [Domain](#2-domain)
  - [CQRS](#3-cqrs)
  - [Persistence](#4-persistence)
  - [Messaging](#5-messaging)
  - [Outbox](#6-outbox)
  - [Multi-Tenancy](#7-multi-tenancy)
  - [Email](#8-email)
- [Complete Integration Example](#complete-integration-example)

## Installation

Add the required building blocks to your project using NuGet:

```bash
dotnet add package Pokok.BuildingBlocks.Domain
dotnet add package Pokok.BuildingBlocks.Cqrs
dotnet add package Pokok.BuildingBlocks.Persistence
dotnet add package Pokok.BuildingBlocks.Messaging
dotnet add package Pokok.BuildingBlocks.Outbox
dotnet add package Pokok.BuildingBlocks.MultiTenancy
dotnet add package Pokok.BuildingBlocks.Common
dotnet add package Pokok.Messaging.Email
```

## Building Blocks Overview

The library consists of eight modular building blocks:

1. **Common** - Shared abstractions (ICurrentUserService)
2. **Domain** - DDD abstractions (Entities, Aggregates, Value Objects, Domain Events)
3. **CQRS** - Command/Query pattern with handlers and dispatchers
4. **Persistence** - Repository pattern, Unit of Work, EF Core abstractions
5. **Messaging** - RabbitMQ message publishing and consuming
6. **Outbox** - Transactional outbox pattern for reliable messaging
7. **Multi-Tenancy** - Multi-tenant application abstractions
8. **Email** - Email message handling and template rendering

## Quick Start

Here's a minimal example showing the most commonly used building blocks together:

```csharp
// 1. Define a domain entity
public class Order : AggregateRoot<Guid>
{
    public string CustomerName { get; private set; }
    public decimal Amount { get; private set; }

    private Order() : base(Guid.NewGuid()) { }

    public static Order Create(string customerName, decimal amount)
    {
        var order = new Order
        {
            CustomerName = customerName,
            Amount = amount
        };
        order.AddDomainEvent(new OrderCreatedEvent(order.Id, customerName, amount));
        return order;
    }
}

public record OrderCreatedEvent(Guid OrderId, string CustomerName, decimal Amount) : DomainEventBase;

// 2. Create a command and handler
public record CreateOrderCommand(string CustomerName, decimal Amount) : ICommand<Guid>;

public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly IRepository<Order> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderHandler(IRepository<Order> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = Order.Create(command.CustomerName, command.Amount);
        await _repository.AddAsync(order);
        await _unitOfWork.CompleteAsync(cancellationToken);
        return order.Id;
    }
}

// 3. Register services in Program.cs
services.AddDbContext<MyDbContext>(options => options.UseSqlServer(connectionString));
services.AddScoped<IRepository<Order>, RepositoryBase<Order, MyDbContext>>();
services.AddScoped<IUnitOfWork>(sp => new UnitOfWork<MyDbContext>(
    sp.GetRequiredService<MyDbContext>(),
    sp.GetService<IDomainEventDispatcher>()
));
services.AddCommandHandler<CreateOrderCommand, Guid, CreateOrderHandler>();

// 4. Use in your application
var orderId = await commandDispatcher.DispatchAsync<CreateOrderCommand, Guid>(
    new CreateOrderCommand("John Doe", 99.99m), cancellationToken);
```

## Detailed Usage by Building Block

### 1. Common

The Common building block provides shared abstractions used across other building blocks.

#### ICurrentUserService

Provides access to the currently authenticated user.

**Interface:**
```csharp
public interface ICurrentUserService
{
    string? UserId { get; }
}
```

**Implementation Example:**
```csharp
public class HttpContextCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}
```

**Registration:**
```csharp
services.AddHttpContextAccessor();
services.AddScoped<ICurrentUserService, HttpContextCurrentUserService>();
```

**Usage:**
The `ICurrentUserService` is automatically used by `DbContextBase` to populate audit fields on entities.

---

### 2. Domain

The Domain building block provides DDD abstractions for entities, aggregates, value objects, and domain events.

#### Entities and Aggregate Roots

**Basic Entity:**
```csharp
public class Product : Entity<Guid>
{
    public string Name { get; private set; }
    public Money Price { get; private set; }

    private Product() : base(Guid.NewGuid()) { }

    public static Product Create(string name, Money price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required");

        return new Product { Name = name, Price = price };
    }

    public void UpdatePrice(Money newPrice)
    {
        if (newPrice.Amount <= 0)
            throw new DomainException("Price must be positive");

        Price = newPrice;
    }
}
```

**Aggregate Root with Domain Events:**
```csharp
public class Order : AggregateRoot<Guid>
{
    private readonly List<OrderItem> _items = new();

    public string CustomerName { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal TotalAmount => _items.Sum(i => i.Amount);

    private Order() : base(Guid.NewGuid()) { }

    public static Order Create(string customerName)
    {
        var order = new Order { CustomerName = customerName };
        order.AddDomainEvent(new OrderCreatedEvent(order.Id));
        return order;
    }

    public void AddItem(string productName, decimal amount)
    {
        _items.Add(new OrderItem(productName, amount));
        AddDomainEvent(new OrderItemAddedEvent(Id, productName, amount));
    }

    public void Complete()
    {
        if (!_items.Any())
            throw new DomainException("Cannot complete order without items");

        AddDomainEvent(new OrderCompletedEvent(Id, TotalAmount));
    }
}

public record OrderCreatedEvent(Guid OrderId) : DomainEventBase;
public record OrderItemAddedEvent(Guid OrderId, string ProductName, decimal Amount) : DomainEventBase;
public record OrderCompletedEvent(Guid OrderId, decimal TotalAmount) : DomainEventBase;
```

#### Value Objects

**Built-in Value Objects:**
```csharp
// Email with validation
var email = new Email("user@example.com");

// Money with currency
var price = new Money(99.99m, "USD");
var total = new Money(199.98m, "USD");
// var invalid = price + new Money(50, "EUR"); // Throws exception - currency mismatch

// Address
var address = new Address(
    street: "123 Main St",
    city: "New York",
    state: "NY",
    postalCode: "10001",
    country: "USA"
);

// Phone number
var phone = new PhoneNumber("+1-555-0123");

// URL
var website = new Url("https://example.com");

// Date range
var dateRange = new DateTimeRange(
    DateTime.UtcNow,
    DateTime.UtcNow.AddDays(30)
);

// Strongly-typed IDs
var userId = new UserId(Guid.NewGuid());
var tenantId = new TenantId(Guid.NewGuid());
```

**Custom Value Objects:**
```csharp
// Simple single-value object
public class ProductCode : SingleValueObject<string>
{
    public ProductCode(string value) : base(value) { }

    protected override void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Product code cannot be empty");

        if (!Regex.IsMatch(value, @"^[A-Z]{3}-\d{4}$"))
            throw new DomainException("Product code must match format: ABC-1234");
    }
}

// Usage
var code = new ProductCode("ABC-1234");
```

**Complex Value Object:**
```csharp
public class Dimensions : ValueObject
{
    public decimal Length { get; }
    public decimal Width { get; }
    public decimal Height { get; }
    public string Unit { get; }

    public Dimensions(decimal length, decimal width, decimal height, string unit = "cm")
    {
        Length = length;
        Width = width;
        Height = height;
        Unit = unit;
        Validate();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Length;
        yield return Width;
        yield return Height;
        yield return Unit;
    }

    protected override void Validate()
    {
        if (Length <= 0 || Width <= 0 || Height <= 0)
            throw new DomainException("Dimensions must be positive");
    }

    public decimal Volume => Length * Width * Height;
}
```

#### Domain Event Handlers

```csharp
public class OrderCreatedEventHandler : IDomainEventHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order {OrderId} was created", domainEvent.OrderId);
        // Additional business logic here
        return Task.CompletedTask;
    }
}

// Registration
services.AddScoped<IDomainEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
```

---

### 3. CQRS

The CQRS building block implements the Command Query Responsibility Segregation pattern.

#### Commands

**Define a Command:**
```csharp
public record CreateOrderCommand(string CustomerName, decimal Amount) : ICommand<Guid>;

public record UpdateOrderCommand(Guid OrderId, string CustomerName) : ICommand<bool>;

public record DeleteOrderCommand(Guid OrderId) : ICommand<bool>;
```

**Command Handler:**
```csharp
public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly IRepository<Order> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrderHandler> _logger;

    public CreateOrderHandler(
        IRepository<Order> repository,
        IUnitOfWork unitOfWork,
        ILogger<CreateOrderHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating order for customer: {CustomerName}", command.CustomerName);

        var order = Order.Create(command.CustomerName, command.Amount);
        await _repository.AddAsync(order);
        await _unitOfWork.CompleteAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} created successfully", order.Id);
        return order.Id;
    }
}
```

**Command Validator:**
```csharp
public class CreateOrderValidator : IValidator<CreateOrderCommand>
{
    public void Validate(CreateOrderCommand request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.CustomerName))
            errors.Add("Customer name is required");

        if (request.Amount <= 0)
            errors.Add("Amount must be greater than zero");

        if (errors.Any())
            throw new ValidationException(errors);
    }
}
```

**Registration:**
```csharp
// With validator
services.AddCommandHandler<CreateOrderCommand, Guid, CreateOrderHandler, CreateOrderValidator>();

// Without validator
services.AddCommandHandler<UpdateOrderCommand, bool, UpdateOrderHandler>();
```

**Usage:**
```csharp
public class OrdersController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public OrdersController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var orderId = await _commandDispatcher.DispatchAsync<CreateOrderCommand, Guid>(
                command, cancellationToken);
            return CreatedAtAction(nameof(GetOrder), new { id = orderId }, orderId);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
    }
}
```

#### Queries

**Define a Query:**
```csharp
public record GetOrderQuery(Guid OrderId) : IQuery<OrderDto>;

public record GetOrdersQuery(int PageNumber, int PageSize) : IQuery<List<OrderDto>>;

public class OrderDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

**Query Handler:**
```csharp
public class GetOrderHandler : IQueryHandler<GetOrderQuery, OrderDto>
{
    private readonly IRepository<Order> _repository;

    public GetOrderHandler(IRepository<Order> repository)
    {
        _repository = repository;
    }

    public async Task<OrderDto> HandleAsync(GetOrderQuery query, CancellationToken cancellationToken)
    {
        var order = await _repository.GetAsync(query.OrderId);

        if (order == null)
            throw new NotFoundException($"Order {query.OrderId} not found");

        return new OrderDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            Amount = order.Amount,
            CreatedAt = order.CreatedAtUtc
        };
    }
}
```

**Registration:**
```csharp
services.AddQueryHandler<GetOrderQuery, OrderDto, GetOrderHandler>();
services.AddQueryHandler<GetOrdersQuery, List<OrderDto>, GetOrdersHandler>();
```

**Usage:**
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetOrder(Guid id, CancellationToken cancellationToken)
{
    var orderDto = await _queryDispatcher.DispatchAsync<GetOrderQuery, OrderDto>(
        new GetOrderQuery(id), cancellationToken);
    return Ok(orderDto);
}
```

#### Domain Event Dispatcher

**Registration:**
```csharp
services.AddDomainEventDispatcher();
services.AddScoped<IDomainEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
```

**Automatic Dispatch:**
When using `UnitOfWork`, domain events are automatically dispatched after `SaveChangesAsync`:

```csharp
// Events are automatically dispatched by UnitOfWork
await _unitOfWork.CompleteAsync(cancellationToken);
```

---

### 4. Persistence

The Persistence building block provides Repository pattern, Unit of Work, and EF Core abstractions.

#### DbContext Setup

**Create Your DbContext:**
```csharp
public class ApplicationDbContext : DbContextBase
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService? currentUserService = null)
        : base(options, currentUserService)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.OwnsOne(e => e.Price, price =>
            {
                price.Property(p => p.Amount).HasColumnName("Price").HasColumnType("decimal(18,2)");
                price.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3);
            });
        });
    }
}
```

**Registration:**
```csharp
services.AddDbContext<ApplicationDbContext>((provider, options) =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    // ICurrentUserService is automatically injected if registered
});
```

#### Repository Pattern

**Using Generic Repository:**
```csharp
public class OrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IRepository<Order> orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Order?> GetOrderAsync(Guid id)
    {
        return await _orderRepository.GetAsync(id);
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _orderRepository.GetAllAsync();
    }

    public async Task<List<Order>> FindOrdersByCustomerAsync(string customerName)
    {
        return await _orderRepository.FindAsync(o => o.CustomerName == customerName);
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        return await _orderRepository.SingleOrDefaultAsync(o => o.Id == id);
    }

    public async Task<bool> OrderExistsAsync(Guid id)
    {
        return await _orderRepository.ExistsAsync(o => o.Id == id);
    }

    public async Task<int> GetTotalOrderCountAsync()
    {
        return await _orderRepository.CountAsync();
    }

    public async Task CreateOrderAsync(Order order)
    {
        await _orderRepository.AddAsync(order);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteOrderAsync(Order order)
    {
        _orderRepository.Remove(order);
        await _unitOfWork.CompleteAsync();
    }
}
```

**Custom Repository:**
```csharp
public interface IOrderRepository : IRepository<Order>
{
    Task<List<Order>> GetRecentOrdersAsync(int count);
    Task<decimal> GetTotalRevenueAsync();
}

public class OrderRepository : RepositoryBase<Order, ApplicationDbContext>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Order>> GetRecentOrdersAsync(int count)
    {
        return await Context.Orders
            .OrderByDescending(o => o.CreatedAtUtc)
            .Take(count)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        return await Context.Orders
            .Where(o => !o.IsDeleted)
            .SumAsync(o => o.Amount);
    }
}

// Registration
services.AddScoped<IOrderRepository, OrderRepository>();
```

#### Specifications

**Define Specifications:**
```csharp
public class ActiveOrdersSpecification : BaseSpecification<Order>
{
    public ActiveOrdersSpecification() : base(order => !order.IsDeleted)
    {
    }
}

public class OrdersByCustomerSpecification : BaseSpecification<Order>
{
    public OrdersByCustomerSpecification(string customerName)
        : base(order => order.CustomerName == customerName && !order.IsDeleted)
    {
    }
}

public class PaginatedOrdersSpecification : BaseSpecification<Order>
{
    public PaginatedOrdersSpecification(int pageNumber, int pageSize)
        : base(order => !order.IsDeleted)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
}
```

**Use Specifications:**
```csharp
public class OrderService
{
    private readonly ApplicationDbContext _context;

    public async Task<List<Order>> GetActiveOrdersAsync()
    {
        var spec = new ActiveOrdersSpecification();
        var query = SpecificationEvaluator.GetQuery(_context.Orders, spec);
        return await query.ToListAsync();
    }

    public async Task<List<Order>> GetOrdersByCustomerAsync(string customerName, int page, int pageSize)
    {
        var customerSpec = new OrdersByCustomerSpecification(customerName);
        var paginationSpec = new PaginatedOrdersSpecification(page, pageSize);

        var combinedSpec = new AndSpecification<Order>(customerSpec, paginationSpec);
        var query = SpecificationEvaluator.GetQuery(_context.Orders, combinedSpec);
        return await query.ToListAsync();
    }
}
```

#### Unit of Work

**Registration:**
```csharp
services.AddScoped<IUnitOfWork>(sp =>
    new UnitOfWork<ApplicationDbContext>(
        sp.GetRequiredService<ApplicationDbContext>(),
        sp.GetService<IDomainEventDispatcher>()
    )
);
```

**Usage:**
```csharp
public class OrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task CreateOrderWithProductsAsync(CreateOrderDto dto)
    {
        var order = Order.Create(dto.CustomerName);

        foreach (var item in dto.Items)
        {
            var product = await _productRepository.GetAsync(item.ProductId);
            if (product == null)
                throw new NotFoundException($"Product {item.ProductId} not found");

            order.AddItem(product.Name, product.Price.Amount * item.Quantity);
        }

        await _orderRepository.AddAsync(order);

        // Single transaction - saves all changes and dispatches domain events
        await _unitOfWork.CompleteAsync();
    }
}
```

#### Auditing and Soft Delete

Entities inheriting from `EntityBase` automatically get audit fields populated:

```csharp
public class Order : EntityBase
{
    public string CustomerName { get; private set; }
    public decimal Amount { get; private set; }

    // CreatedAtUtc, CreatedBy, ModifiedAtUtc, ModifiedBy, IsDeleted, DeletedAtUtc, DeletedBy
    // are automatically managed by DbContextBase
}

// Soft delete
_orderRepository.Remove(order); // Sets IsDeleted = true instead of deleting
await _unitOfWork.CompleteAsync();

// Query automatically filters soft-deleted entities
var orders = await _orderRepository.GetAllAsync(); // Only returns non-deleted orders
```

---

### 5. Messaging

The Messaging building block provides RabbitMQ integration for publishing and consuming messages.

#### Configuration

**Setup RabbitMQ Options:**
```csharp
// appsettings.json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest"
  }
}

// Program.cs
services.Configure<RabbitMQOptions>(configuration.GetSection("RabbitMQ"));
```

#### Publishing Messages

**Register Publisher:**
```csharp
services.AddScoped<IRabbitMQConnection, RabbitMQConnection>();
services.AddScoped<IMessagePublisher>(sp =>
    new RabbitMQMessagePublisher(
        sp.GetRequiredService<IRabbitMQConnection>(),
        sp.GetRequiredService<ILogger<RabbitMQMessagePublisher>>(),
        exchangeName: "myapp.exchange"
    )
);
```

**Publish Messages:**
```csharp
public class OrderEventPublisher
{
    private readonly IMessagePublisher _messagePublisher;

    public OrderEventPublisher(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task PublishOrderCreatedAsync(Order order, CancellationToken cancellationToken)
    {
        var message = new OrderCreatedMessage
        {
            OrderId = order.Id,
            CustomerName = order.CustomerName,
            Amount = order.Amount,
            CreatedAt = DateTime.UtcNow
        };

        var payload = JsonSerializer.Serialize(message);
        await _messagePublisher.PublishAsync("OrderCreated", payload, cancellationToken);
    }
}
```

#### Consuming Messages

**Define Message Handler:**
```csharp
public class OrderCreatedMessage
{
    public Guid OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrderCreatedMessageHandler : IRabbitMQMessageHandler<OrderCreatedMessage>
{
    private readonly ILogger<OrderCreatedMessageHandler> _logger;

    public OrderCreatedMessageHandler(ILogger<OrderCreatedMessageHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(OrderCreatedMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Processing order created: {OrderId} for {CustomerName}",
            message.OrderId,
            message.CustomerName);

        // Process the message (e.g., send notification, update analytics, etc.)
        await Task.CompletedTask;
    }
}
```

**Register Consumer:**
```csharp
services.AddScoped<IRabbitMQMessageHandler<OrderCreatedMessage>, OrderCreatedMessageHandler>();
services.AddHostedService(sp =>
    new RabbitMQMessageConsumer<OrderCreatedMessage>(
        sp.GetRequiredService<IRabbitMQConnection>(),
        sp.GetRequiredService<IRabbitMQMessageHandler<OrderCreatedMessage>>(),
        sp.GetRequiredService<ILogger<RabbitMQMessageConsumer<OrderCreatedMessage>>>(),
        queueName: "order.created.queue",
        routingKey: "OrderCreated",
        exchangeName: "myapp.exchange"
    )
);
```

---

### 6. Outbox

The Outbox building block implements the transactional outbox pattern for reliable messaging.

#### Setup

**Add Outbox Table to DbContext:**
```csharp
public class ApplicationDbContext : DbContextBase
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("OutboxMessages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Payload).IsRequired();
            entity.Property(e => e.SourceApp).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Error).HasMaxLength(1000);
            entity.HasIndex(e => e.ProcessedOnUtc);
        });
    }
}
```

**Register Outbox Processor:**
```csharp
// Configure polling interval
services.Configure<OutboxOptions>(options =>
{
    options.Interval = TimeSpan.FromSeconds(5);
});

// Add outbox processor
services.AddOutboxProcessor<ApplicationDbContext>();
```

#### Usage

**Add Messages to Outbox:**
```csharp
public class OrderCreatedEventHandler : IDomainEventHandler<OrderCreatedEvent>
{
    private readonly IOutboxMessageRepository _outboxRepository;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(
        IOutboxMessageRepository outboxRepository,
        ILogger<OrderCreatedEventHandler> logger)
    {
        _outboxRepository = outboxRepository;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling OrderCreatedEvent for order {OrderId}", domainEvent.OrderId);

        // Create notification message
        var notification = new OrderNotificationMessage
        {
            OrderId = domainEvent.OrderId,
            EventType = "OrderCreated",
            Timestamp = DateTime.UtcNow
        };

        // Add to outbox (will be processed by background service)
        var outboxMessage = new OutboxMessage(
            type: OutboxMessageType.Custom("OrderNotification"),
            payload: JsonSerializer.Serialize(notification),
            sourceApp: "OrderService",
            occurredOnUtc: domainEvent.OccurredOn
        );

        await _outboxRepository.AddAsync(outboxMessage, cancellationToken);
        await _outboxRepository.CompleteAsync(cancellationToken);
    }
}
```

**How It Works:**

1. Domain events trigger handlers that add messages to the outbox table
2. Messages are saved in the same transaction as your domain changes
3. Background `OutboxProcessorHostedService` polls for unprocessed messages
4. Messages are published to the message broker (RabbitMQ)
5. Successfully published messages are marked as processed
6. Failed messages are marked with errors and retried later

**Benefits:**

- Guaranteed message delivery (at-least-once semantics)
- Messages survive broker downtime
- Transactional consistency between domain changes and messaging
- Automatic retry on failures

---

### 7. Multi-Tenancy

The Multi-Tenancy building block provides abstractions for multi-tenant applications.

#### Setup

**Implement Tenant Provider:**
```csharp
public class HttpContextTenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextTenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public TenantId? GetCurrentTenantId()
    {
        // Get tenant from header
        var tenantHeader = _httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault();

        if (Guid.TryParse(tenantHeader, out var tenantGuid))
            return new TenantId(tenantGuid);

        // Alternatively, get from claims
        var tenantClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value;
        if (Guid.TryParse(tenantClaim, out var claimTenantGuid))
            return new TenantId(claimTenantGuid);

        return null;
    }
}
```

**Registration:**
```csharp
services.AddHttpContextAccessor();
services.AddScoped<ITenantProvider, HttpContextTenantProvider>();
```

#### Tenant-Scoped Entities

**Define Tenant-Scoped Entity:**
```csharp
public class Order : EntityBase, ITenantScoped
{
    public TenantId TenantId { get; private set; }
    public string CustomerName { get; private set; }
    public decimal Amount { get; private set; }

    private Order() { }

    public static Order Create(TenantId tenantId, string customerName, decimal amount)
    {
        return new Order
        {
            TenantId = tenantId,
            CustomerName = customerName,
            Amount = amount
        };
    }
}
```

**Configure in DbContext:**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Order>(entity =>
    {
        entity.Property(e => e.TenantId)
            .HasConversion(
                v => v.Value,
                v => new TenantId(v))
            .IsRequired();

        entity.HasIndex(e => e.TenantId);
    });
}
```

#### Automatic Tenant Filtering

**Create Query Filter:**
```csharp
public class ApplicationDbContext : DbContextBase
{
    private readonly ITenantProvider? _tenantProvider;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService? currentUserService = null,
        ITenantProvider? tenantProvider = null)
        : base(options, currentUserService)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply global query filter for tenant isolation
        modelBuilder.Entity<Order>().HasQueryFilter(o =>
            _tenantProvider == null ||
            _tenantProvider.GetCurrentTenantId() == null ||
            o.TenantId == _tenantProvider.GetCurrentTenantId());
    }
}
```

**Usage:**
```csharp
public class OrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Guid> CreateOrderAsync(string customerName, decimal amount)
    {
        var tenantId = _tenantProvider.GetCurrentTenantId()
            ?? throw new InvalidOperationException("Tenant not found");

        var order = Order.Create(tenantId, customerName, amount);
        await _orderRepository.AddAsync(order);
        await _unitOfWork.CompleteAsync();

        return order.Id;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        // Automatically filtered by current tenant
        return await _orderRepository.GetAllAsync();
    }
}
```

---

### 8. Email

The Email building block provides email message handling and template rendering.

#### Template Rendering

**Define Template:**
```csharp
public class EmailTemplateService
{
    private readonly ITemplateRenderer _templateRenderer;

    public EmailTemplateService(ITemplateRenderer templateRenderer)
    {
        _templateRenderer = templateRenderer;
    }

    public (string Subject, string Body) RenderWelcomeEmail(string userName, string activationLink)
    {
        var template = new EmailTemplateOptions
        {
            Subject = "Welcome to {AppName}, {UserName}!",
            Body = @"
                <html>
                <body>
                    <h1>Welcome {UserName}!</h1>
                    <p>Thank you for joining {AppName}.</p>
                    <p>Please click the link below to activate your account:</p>
                    <a href='{ActivationLink}'>Activate Account</a>
                    <p>Best regards,<br/>The {AppName} Team</p>
                </body>
                </html>"
        };

        var model = new
        {
            AppName = "MyApp",
            UserName = userName,
            ActivationLink = activationLink
        };

        return _templateRenderer.Render(template, model);
    }
}
```

**Registration:**
```csharp
services.AddScoped<ITemplateRenderer, SimpleTemplateRenderer>();
```

#### Email Dispatch Messages

**Create and Send Email:**
```csharp
public class EmailService
{
    private readonly ITemplateRenderer _templateRenderer;
    private readonly IOutboxMessageRepository _outboxRepository;

    public async Task SendWelcomeEmailAsync(string userEmail, string userName, string activationLink)
    {
        // Render template
        var (subject, body) = RenderWelcomeEmail(userName, activationLink);

        // Create email message
        var emailMessage = new EmailDispatchMessage
        {
            To = new List<string> { userEmail },
            Subject = subject,
            Body = body,
            IsHtml = true,
            TemplateKey = "welcome_email"
        };

        // Add to outbox for reliable delivery
        var outboxMessage = new OutboxMessage(
            type: OutboxMessageType.Email,
            payload: JsonSerializer.Serialize(emailMessage),
            sourceApp: "UserService",
            occurredOnUtc: DateTime.UtcNow
        );

        await _outboxRepository.AddAsync(outboxMessage);
        await _outboxRepository.CompleteAsync();
    }
}
```

**Email Consumer (in separate email service):**
```csharp
public class EmailMessageHandler : IRabbitMQMessageHandler<EmailDispatchMessage>
{
    private readonly IEmailSender _emailSender; // Your SMTP implementation
    private readonly ILogger<EmailMessageHandler> _logger;

    public EmailMessageHandler(IEmailSender emailSender, ILogger<EmailMessageHandler> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task HandleAsync(EmailDispatchMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending email to {Recipients}", string.Join(", ", message.To));

        try
        {
            await _emailSender.SendEmailAsync(
                to: message.To,
                cc: message.Cc,
                bcc: message.Bcc,
                subject: message.Subject,
                body: message.Body,
                isHtml: message.IsHtml
            );

            _logger.LogInformation("Email sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email");
            throw;
        }
    }
}

// Registration
services.AddHostedService(sp =>
    new RabbitMQMessageConsumer<EmailDispatchMessage>(
        sp.GetRequiredService<IRabbitMQConnection>(),
        sp.GetRequiredService<IRabbitMQMessageHandler<EmailDispatchMessage>>(),
        sp.GetRequiredService<ILogger<RabbitMQMessageConsumer<EmailDispatchMessage>>>(),
        queueName: "email.queue",
        routingKey: "Email",
        exchangeName: "myapp.exchange"
    )
);
```

---

## Complete Integration Example

Here's a complete example showing all building blocks working together in a minimal API application:

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// 1. Common - Current User Service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, HttpContextCurrentUserService>();

// 2. Multi-Tenancy
builder.Services.AddScoped<ITenantProvider, HttpContextTenantProvider>();

// 3. Persistence - DbContext
builder.Services.AddDbContext<ApplicationDbContext>((provider, options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// 4. Persistence - Repositories and Unit of Work
builder.Services.AddScoped<IRepository<Order>, RepositoryBase<Order, ApplicationDbContext>>();
builder.Services.AddScoped<IUnitOfWork>(sp =>
    new UnitOfWork<ApplicationDbContext>(
        sp.GetRequiredService<ApplicationDbContext>(),
        sp.GetService<IDomainEventDispatcher>()
    )
);

// 5. CQRS - Command/Query Handlers
builder.Services.AddCommandHandler<CreateOrderCommand, Guid, CreateOrderHandler, CreateOrderValidator>();
builder.Services.AddQueryHandler<GetOrderQuery, OrderDto, GetOrderHandler>();
builder.Services.AddDomainEventDispatcher();
builder.Services.AddScoped<IDomainEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();

// 6. Messaging - RabbitMQ
builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddScoped<IRabbitMQConnection, RabbitMQConnection>();
builder.Services.AddScoped<IMessagePublisher>(sp =>
    new RabbitMQMessagePublisher(
        sp.GetRequiredService<IRabbitMQConnection>(),
        sp.GetRequiredService<ILogger<RabbitMQMessagePublisher>>(),
        "myapp.exchange"
    )
);

// 7. Outbox - Reliable Messaging
builder.Services.Configure<OutboxOptions>(options =>
{
    options.Interval = TimeSpan.FromSeconds(10);
});
builder.Services.AddOutboxProcessor<ApplicationDbContext>();

// 8. Email - Template Rendering
builder.Services.AddScoped<ITemplateRenderer, SimpleTemplateRenderer>();

var app = builder.Build();

// API Endpoints
app.MapPost("/orders", async (
    CreateOrderCommand command,
    ICommandDispatcher commandDispatcher,
    CancellationToken cancellationToken) =>
{
    try
    {
        var orderId = await commandDispatcher.DispatchAsync<CreateOrderCommand, Guid>(
            command, cancellationToken);
        return Results.Created($"/orders/{orderId}", orderId);
    }
    catch (ValidationException ex)
    {
        return Results.BadRequest(new { errors = ex.Errors });
    }
});

app.MapGet("/orders/{id}", async (
    Guid id,
    IQueryDispatcher queryDispatcher,
    CancellationToken cancellationToken) =>
{
    var order = await queryDispatcher.DispatchAsync<GetOrderQuery, OrderDto>(
        new GetOrderQuery(id), cancellationToken);
    return order != null ? Results.Ok(order) : Results.NotFound();
});

app.Run();
```

This example demonstrates:
- Multi-tenant order management
- Automatic audit tracking
- CQRS pattern with validation
- Domain events and handlers
- Transactional outbox pattern
- Message publishing to RabbitMQ
- Email notifications via templates
- Repository and Unit of Work patterns
- Soft delete support

## Summary

The Pokok Building Blocks library provides a complete set of abstractions and implementations for building robust .NET applications following best practices:

- **Modular Design**: Use only the building blocks you need
- **Clean Architecture**: Clear separation of concerns
- **Domain-Driven Design**: Rich domain models with behavior
- **CQRS**: Separate read and write models
- **Event-Driven**: Domain events and reliable messaging
- **Multi-Tenancy**: Built-in tenant isolation
- **Production-Ready**: Audit logging, soft deletes, error handling

For more information, see the [example application](../examples/OrderManagement/) and individual building block source code.
