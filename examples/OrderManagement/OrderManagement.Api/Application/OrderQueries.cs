using OrderManagement.Api.Domain;
using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.BuildingBlocks.Persistence.Abstractions;

namespace OrderManagement.Api.Application;

/// <summary>
/// Query to get a single order by ID
/// </summary>
public record GetOrderQuery(Guid OrderId) : IQuery<OrderDto>;

/// <summary>
/// Query to get all orders
/// </summary>
public record GetAllOrdersQuery : IQuery<List<OrderDto>>;

/// <summary>
/// Data Transfer Object for Order
/// </summary>
public class OrderDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
}

/// <summary>
/// Handler for GetOrderQuery
/// </summary>
public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, OrderDto>
{
    private readonly IRepository<Order> _repository;

    public GetOrderQueryHandler(IRepository<Order> repository)
    {
        _repository = repository;
    }

    public async Task<OrderDto> HandleAsync(GetOrderQuery query, CancellationToken cancellationToken)
    {
        var order = await _repository.GetAsync(query.OrderId);

        if (order == null)
            throw new KeyNotFoundException($"Order {query.OrderId} not found");

        return new OrderDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            Amount = order.Amount,
            CreatedAtUtc = order.CreatedAtUtc,
            ModifiedAtUtc = order.ModifiedAtUtc
        };
    }
}

/// <summary>
/// Handler for GetAllOrdersQuery
/// </summary>
public class GetAllOrdersQueryHandler : IQueryHandler<GetAllOrdersQuery, List<OrderDto>>
{
    private readonly IRepository<Order> _repository;

    public GetAllOrdersQueryHandler(IRepository<Order> repository)
    {
        _repository = repository;
    }

    public async Task<List<OrderDto>> HandleAsync(GetAllOrdersQuery query, CancellationToken cancellationToken)
    {
        var orders = await _repository.GetAllAsync();

        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            Amount = order.Amount,
            CreatedAtUtc = order.CreatedAtUtc,
            ModifiedAtUtc = order.ModifiedAtUtc
        }).ToList();
    }
}
