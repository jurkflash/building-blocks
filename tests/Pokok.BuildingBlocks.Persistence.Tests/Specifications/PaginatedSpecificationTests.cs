using Pokok.BuildingBlocks.Persistence.Specifications.Core;
using Xunit;

namespace Pokok.BuildingBlocks.Persistence.Specifications;

internal sealed class PagedProductSpec : PaginatedSpecification<Product>
{
    public PagedProductSpec(string category, int pageNumber, int pageSize)
        : base(p => p.Category == category, pageNumber, pageSize)
    {
    }
}

public class PaginatedSpecificationTests
{
    [Fact]
    public void Constructor_WithValidPageAndSize_SetsPageNumberAndSize()
    {
        var spec = new PagedProductSpec("Electronics", 2, 10);

        Assert.Equal(2, spec.PageNumber);
        Assert.Equal(10, spec.PageSize);
    }

    [Fact]
    public void Constructor_WithPageOneAndSize10_SetsSkipToZero()
    {
        var spec = new PagedProductSpec("Electronics", 1, 10);

        Assert.Equal(0, spec.Skip);
        Assert.Equal(10, spec.Take);
    }

    [Fact]
    public void Constructor_WithPageTwoAndSize5_SetsSkipToFive()
    {
        var spec = new PagedProductSpec("Electronics", 2, 5);

        Assert.Equal(5, spec.Skip);
        Assert.Equal(5, spec.Take);
    }

    [Fact]
    public void Constructor_WithZeroPageNumber_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new PagedProductSpec("Electronics", 0, 10));
    }

    [Fact]
    public void Constructor_WithNegativePageNumber_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new PagedProductSpec("Electronics", -1, 10));
    }

    [Fact]
    public void Constructor_WithZeroPageSize_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new PagedProductSpec("Electronics", 1, 0));
    }

    [Fact]
    public void Constructor_WithNegativePageSize_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new PagedProductSpec("Electronics", 1, -1));
    }
}
