using NSubstitute;
using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;
using Pokok.BuildingBlocks.Persistence.Specifications.Core;
using Pokok.BuildingBlocks.Persistence.Specifications.Evaluator;
using Pokok.BuildingBlocks.Persistence.Specifications.Handlers;
using Xunit;

namespace Pokok.BuildingBlocks.Persistence.Specifications;

public class FilteringSpecificationHandlerTests
{
    private readonly IQueryable<Product> _query = new List<Product>
    {
        new() { Id = 1, Category = "A", IsActive = true },
        new() { Id = 2, Category = "B", IsActive = false },
        new() { Id = 3, Category = "A", IsActive = false },
    }.AsQueryable();

    [Fact]
    public void Apply_WithMatchingSpec_FiltersQuery()
    {
        var handler = new FilteringSpecificationHandler<Product>();
        var spec = new CategorySpec("A");

        var result = handler.Apply(_query, spec).ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.Equal("A", p.Category));
    }

    [Fact]
    public void Apply_WithNoMatch_ReturnsEmptyQuery()
    {
        var handler = new FilteringSpecificationHandler<Product>();
        var spec = new CategorySpec("Z");

        var result = handler.Apply(_query, spec).ToList();

        Assert.Empty(result);
    }
}

public class OrderSpecificationHandlerTests
{
    private readonly IQueryable<Product> _query = new List<Product>
    {
        new() { Id = 3, Category = "C", Price = 30m },
        new() { Id = 1, Category = "A", Price = 10m },
        new() { Id = 2, Category = "B", Price = 20m },
    }.AsQueryable();

    [Fact]
    public void Apply_WithOrderBySpec_OrdersAscending()
    {
        var handler = new OrderSpecificationHandler<Product>();
        var spec = Substitute.For<ISpecification<Product>, IOrderSpecification<Product>>();
        ((IOrderSpecification<Product>)spec).OrderBy.Returns(p => (object)p.Price);
        ((IOrderSpecification<Product>)spec).OrderByDescending.Returns((System.Linq.Expressions.Expression<Func<Product, object>>?)null);

        var result = handler.Apply(_query, spec).ToList();

        Assert.Equal(10m, result[0].Price);
        Assert.Equal(20m, result[1].Price);
        Assert.Equal(30m, result[2].Price);
    }

    [Fact]
    public void Apply_WithOrderByDescendingSpec_OrdersDescending()
    {
        var handler = new OrderSpecificationHandler<Product>();
        var spec = Substitute.For<ISpecification<Product>, IOrderSpecification<Product>>();
        ((IOrderSpecification<Product>)spec).OrderBy.Returns((System.Linq.Expressions.Expression<Func<Product, object>>?)null);
        ((IOrderSpecification<Product>)spec).OrderByDescending.Returns(p => (object)p.Price);

        var result = handler.Apply(_query, spec).ToList();

        Assert.Equal(30m, result[0].Price);
        Assert.Equal(20m, result[1].Price);
        Assert.Equal(10m, result[2].Price);
    }

    [Fact]
    public void Apply_WithNonOrderSpec_ReturnsQueryUnchanged()
    {
        var handler = new OrderSpecificationHandler<Product>();
        var spec = new CategorySpec("A");

        var result = handler.Apply(_query, spec).ToList();

        Assert.Equal(3, result.Count);
    }
}

public class PagingSpecificationHandlerTests
{
    private readonly IQueryable<Product> _query = Enumerable.Range(1, 20)
        .Select(i => new Product { Id = i, Category = "A" })
        .AsQueryable();

    [Fact]
    public void Apply_WithPagingEnabled_SkipsAndTakes()
    {
        var handler = new PagingSpecificationHandler<Product>();
        var spec = Substitute.For<ISpecification<Product>, IPagingSpecification>();
        ((IPagingSpecification)spec).Skip.Returns(5);
        ((IPagingSpecification)spec).Take.Returns(3);
        ((IPagingSpecification)spec).IsPagingEnabled.Returns(true);

        var result = handler.Apply(_query, spec).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal(6, result[0].Id);
    }

    [Fact]
    public void Apply_WithPagingDisabled_ReturnsAllItems()
    {
        var handler = new PagingSpecificationHandler<Product>();
        var spec = Substitute.For<ISpecification<Product>, IPagingSpecification>();
        ((IPagingSpecification)spec).IsPagingEnabled.Returns(false);

        var result = handler.Apply(_query, spec).ToList();

        Assert.Equal(20, result.Count);
    }

    [Fact]
    public void Apply_WithNonPagingSpec_ReturnsQueryUnchanged()
    {
        var handler = new PagingSpecificationHandler<Product>();
        var spec = new CategorySpec("A");

        var result = handler.Apply(_query, spec).ToList();

        Assert.Equal(20, result.Count);
    }
}

public class IncludeSpecificationHandlerTests
{
    private readonly IQueryable<Product> _query = new List<Product>
    {
        new() { Id = 1, Category = "A" },
    }.AsQueryable();

    [Fact]
    public void Apply_WithNonIncludeSpec_ReturnsQueryUnchanged()
    {
        var handler = new IncludeSpecificationHandler<Product>();
        var spec = new CategorySpec("A");

        var result = handler.Apply(_query, spec).ToList();

        Assert.Single(result);
    }
}

public class SpecificationEvaluatorTests
{
    private readonly IQueryable<Product> _query = new List<Product>
    {
        new() { Id = 1, Category = "A", IsActive = true, Price = 100m },
        new() { Id = 2, Category = "B", IsActive = true, Price = 200m },
        new() { Id = 3, Category = "A", IsActive = false, Price = 50m },
    }.AsQueryable();

    [Fact]
    public void GetQuery_WithFilteringHandler_ReturnsFilteredResults()
    {
        var evaluator = new SpecificationEvaluator<Product>(
            new ISpecificationHandler<Product>[] { new FilteringSpecificationHandler<Product>() });
        var spec = new CategorySpec("A");

        var result = evaluator.GetQuery(_query, spec).ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.Equal("A", p.Category));
    }

    [Fact]
    public void GetQuery_WithMultipleHandlers_AppliesAllHandlers()
    {
        var evaluator = new SpecificationEvaluator<Product>(new ISpecificationHandler<Product>[]
        {
            new FilteringSpecificationHandler<Product>(),
            new OrderSpecificationHandler<Product>(),
        });
        var spec = new ActiveSpec();

        var result = evaluator.GetQuery(_query, spec).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetQuery_WithNoHandlers_ReturnsOriginalQuery()
    {
        var evaluator = new SpecificationEvaluator<Product>(Enumerable.Empty<ISpecificationHandler<Product>>());
        var spec = new CategorySpec("A");

        var result = evaluator.GetQuery(_query, spec).ToList();

        Assert.Equal(3, result.Count);
    }
}
