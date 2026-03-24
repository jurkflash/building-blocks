using Pokok.BuildingBlocks.Persistence.Specifications.Core;
using Pokok.BuildingBlocks.Persistence.Specifications.Extensions;
using Xunit;

namespace Pokok.BuildingBlocks.Persistence.Specifications;

internal sealed class CategorySpec : BaseSpecification<Product>
{
    public CategorySpec(string category) : base(p => p.Category == category) { }
}

internal sealed class ActiveSpec : BaseSpecification<Product>
{
    public ActiveSpec() : base(p => p.IsActive) { }
}

internal sealed class ExpensiveSpec : BaseSpecification<Product>
{
    public ExpensiveSpec(decimal minPrice) : base(p => p.Price > minPrice) { }
}

public class ExpressionExtensionsTests
{
    private readonly List<Product> _products = new()
    {
        new Product { Id = 1, Category = "Electronics", IsActive = true, Price = 500m },
        new Product { Id = 2, Category = "Electronics", IsActive = false, Price = 100m },
        new Product { Id = 3, Category = "Clothing", IsActive = true, Price = 50m },
        new Product { Id = 4, Category = "Clothing", IsActive = false, Price = 200m },
    };

    [Fact]
    public void AndAlso_CombinesTwoPredicates_ReturnsItemsMatchingBoth()
    {
        var electronicsSpec = new CategorySpec("Electronics");
        var activeSpec = new ActiveSpec();

        var combined = electronicsSpec.Criteria.AndAlso(activeSpec.Criteria);
        var result = _products.Where(combined.Compile()).ToList();

        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
    }

    [Fact]
    public void OrElse_CombinesTwoPredicates_ReturnsItemsMatchingEither()
    {
        var electronicsSpec = new CategorySpec("Electronics");
        var activeSpec = new ActiveSpec();

        var combined = electronicsSpec.Criteria.OrElse(activeSpec.Criteria);
        var result = _products.Where(combined.Compile()).ToList();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Not_NegatesExpression_ReturnsItemsNotMatching()
    {
        var electronicsSpec = new CategorySpec("Electronics");

        var negated = electronicsSpec.Criteria.Not();
        var result = _products.Where(negated.Compile()).ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.Equal("Clothing", p.Category));
    }
}

public class AndSpecificationTests
{
    private readonly List<Product> _products = new()
    {
        new Product { Id = 1, Category = "Electronics", IsActive = true },
        new Product { Id = 2, Category = "Electronics", IsActive = false },
        new Product { Id = 3, Category = "Clothing", IsActive = true },
    };

    [Fact]
    public void AndSpecification_WithTwoSpecs_MatchesOnlyBoth()
    {
        var left = new CategorySpec("Electronics");
        var right = new ActiveSpec();
        var spec = new AndSpecification<Product>(left, right);

        var result = _products.Where(spec.ToPredicate()).ToList();

        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
    }

    [Fact]
    public void AndSpecification_Criteria_IsNotNull()
    {
        var spec = new AndSpecification<Product>(new CategorySpec("X"), new ActiveSpec());

        Assert.NotNull(spec.Criteria);
    }
}

public class OrSpecificationTests
{
    private readonly List<Product> _products = new()
    {
        new Product { Id = 1, Category = "Electronics", IsActive = false, Price = 50m },
        new Product { Id = 2, Category = "Clothing", IsActive = true, Price = 50m },
        new Product { Id = 3, Category = "Books", IsActive = false, Price = 50m },
    };

    [Fact]
    public void OrSpecification_WithTwoSpecs_MatchesEither()
    {
        var left = new CategorySpec("Electronics");
        var right = new ActiveSpec();
        var spec = new OrSpecification<Product>(left, right);

        var result = _products.Where(spec.ToPredicate()).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void OrSpecification_Criteria_IsNotNull()
    {
        var spec = new OrSpecification<Product>(new CategorySpec("X"), new ActiveSpec());

        Assert.NotNull(spec.Criteria);
    }
}

public class NotSpecificationTests
{
    private readonly List<Product> _products = new()
    {
        new Product { Id = 1, Category = "Electronics", IsActive = true },
        new Product { Id = 2, Category = "Clothing", IsActive = true },
        new Product { Id = 3, Category = "Books", IsActive = true },
    };

    [Fact]
    public void NotSpecification_WithSpec_MatchesOpposite()
    {
        var spec = new NotSpecification<Product>(new CategorySpec("Electronics"));

        var result = _products.Where(spec.ToPredicate()).ToList();

        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, p => p.Category == "Electronics");
    }

    [Fact]
    public void NotSpecification_Criteria_IsNotNull()
    {
        var spec = new NotSpecification<Product>(new CategorySpec("X"));

        Assert.NotNull(spec.Criteria);
    }
}
