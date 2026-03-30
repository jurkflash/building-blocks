using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Application;
using OrderManagement.Api.Domain;
using OrderManagement.Api.Infrastructure;
using Pokok.BuildingBlocks.Common;
using Pokok.BuildingBlocks.Cqrs.Dispatching;
using Pokok.BuildingBlocks.Cqrs.Events;
using Pokok.BuildingBlocks.Cqrs.Extensions;
using Pokok.BuildingBlocks.Cqrs.Validation;
using Pokok.BuildingBlocks.Domain.Events;
using Pokok.BuildingBlocks.Persistence;
using Pokok.BuildingBlocks.Persistence.Abstractions;
using Pokok.BuildingBlocks.Persistence.Base;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Common - Current User Service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, SimpleCurrentUserService>();

// 2. Persistence - DbContext with In-Memory Database
builder.Services.AddDbContext<OrderDbContext>((provider, options) =>
{
    options.UseInMemoryDatabase("OrderManagementDb");
});

// 3. Persistence - Repositories and Unit of Work
builder.Services.AddScoped<IRepository<Order>, RepositoryBase<Order, OrderDbContext>>();
builder.Services.AddScoped<IUnitOfWork>(sp =>
    new UnitOfWork<OrderDbContext>(
        sp.GetRequiredService<OrderDbContext>(),
        sp.GetService<IDomainEventDispatcher>()
    )
);

// 4. CQRS - Command/Query Handlers
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();
builder.Services.AddCommandHandler<CreateOrderCommand, Guid, CreateOrderCommandHandler, CreateOrderCommandValidator>();
builder.Services.AddQueryHandler<GetOrderQuery, OrderDto, GetOrderQueryHandler>();
builder.Services.AddQueryHandler<GetAllOrdersQuery, List<OrderDto>, GetAllOrdersQueryHandler>();
builder.Services.AddDomainEventDispatcher();
builder.Services.AddScoped<IDomainEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// API Endpoints
app.MapPost("/api/orders", async (
    CreateOrderCommand command,
    ICommandDispatcher commandDispatcher,
    CancellationToken cancellationToken) =>
{
    try
    {
        var orderId = await commandDispatcher.DispatchAsync<CreateOrderCommand, Guid>(
            command, cancellationToken);
        return Results.Created($"/api/orders/{orderId}", new { id = orderId });
    }
    catch (ValidationException ex)
    {
        return Results.BadRequest(new { errors = ex.Errors });
    }
})
.WithName("CreateOrder")
.WithOpenApi();

app.MapGet("/api/orders", async (
    IQueryDispatcher queryDispatcher,
    CancellationToken cancellationToken) =>
{
    var orders = await queryDispatcher.DispatchAsync<GetAllOrdersQuery, List<OrderDto>>(
        new GetAllOrdersQuery(), cancellationToken);
    return Results.Ok(orders);
})
.WithName("GetAllOrders")
.WithOpenApi();

app.MapGet("/api/orders/{id}", async (
    Guid id,
    IQueryDispatcher queryDispatcher,
    CancellationToken cancellationToken) =>
{
    try
    {
        var order = await queryDispatcher.DispatchAsync<GetOrderQuery, OrderDto>(
            new GetOrderQuery(id), cancellationToken);
        return Results.Ok(order);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound(new { message = $"Order {id} not found" });
    }
})
.WithName("GetOrder")
.WithOpenApi();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
