using Pokok.BuildingBlocks.Persistence.Specifications.Core;
using Xunit;

namespace Pokok.BuildingBlocks.Persistence.Specifications;

internal sealed class ProductSpecification : BaseSpecification<Product>
{
    public ProductSpecification(string category)
        : base(p => p.Category == category)
    {
    }
}

internal sealed class PagedProductSpecification : BaseSpecification<Product>
{
    public PagedProductSpecification(string category, int skip, int take)
        : base(p => p.Category == category)
    {
        ApplyPaging(skip, take);
    }
}

public class BaseSpecificationTests
{
    [Fact]
    public void ToPredicate_WithMatchingProduct_ReturnsTrue()
    {
        var spec = new ProductSpecification("Electronics");

        var predicate = spec.ToPredicate();

        Assert.True(predicate(new Product { Category = "Electronics" }));
    }

    [Fact]
    public void ToPredicate_WithNonMatchingProduct_ReturnsFalse()
    {
        var spec = new ProductSpecification("Electronics");

        var predicate = spec.ToPredicate();

        Assert.False(predicate(new Product { Category = "Clothing" }));
    }

    [Fact]
    public void Criteria_WhenCreated_ReturnsExpression()
    {
        var spec = new ProductSpecification("Electronics");

        Assert.NotNull(spec.Criteria);
    }

    [Fact]
    public void ApplyPaging_WithValidSkipAndTake_SetsSkipAndTake()
    {
        var spec = new PagedProductSpecification("Electronics", 10, 5);

        Assert.Equal(10, spec.Skip);
        Assert.Equal(5, spec.Take);
    }

    [Fact]
    public void ApplyPaging_WithNegativeSkip_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new PagedProductSpecification("Electronics", -1, 5));
    }

    [Fact]
    public void ApplyPaging_WithZeroTake_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new PagedProductSpecification("Electronics", 0, 0));
    }
}
