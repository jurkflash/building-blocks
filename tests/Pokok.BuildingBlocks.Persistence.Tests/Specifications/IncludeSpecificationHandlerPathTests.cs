using Microsoft.EntityFrameworkCore;
using Pokok.BuildingBlocks.Persistence.Entities;
using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;
using Pokok.BuildingBlocks.Persistence.Specifications.Core;
using Pokok.BuildingBlocks.Persistence.Specifications.Handlers;
using System.Linq.Expressions;
using Xunit;

namespace Pokok.BuildingBlocks.Persistence.Specifications;

internal class OrderEntity : EntityBase
{
    public string Reference { get; set; } = string.Empty;
    public List<OrderLineEntity> Lines { get; set; } = new();
}

internal class OrderLineEntity : EntityBase
{
    public string ProductName { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public OrderEntity? Order { get; set; }
}

internal class OrderContext : DbContext
{
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderLineEntity> OrderLines => Set<OrderLineEntity>();

    public OrderContext(DbContextOptions options) : base(options) { }
}

internal sealed class OrdersWithLinesSpec : BaseSpecification<OrderEntity>, IIncludeSpecification<OrderEntity>
{
    public List<Expression<Func<OrderEntity, object>>> Includes { get; } = new()
    {
        o => o.Lines
    };

    public OrdersWithLinesSpec() : base(o => true) { }
}

public class IncludeSpecificationHandlerIncludePathTests
{
    [Fact]
    public void Apply_WithIncludeSpecification_ExecutesIncludePath()
    {
        var options = new DbContextOptionsBuilder<OrderContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new OrderContext(options);

        var handler = new IncludeSpecificationHandler<OrderEntity>();
        var spec = new OrdersWithLinesSpec();

        var query = context.Orders.AsQueryable();
        var result = handler.Apply(query, spec);

        Assert.NotNull(result);
    }
}
