using Pokok.BuildingBlocks.Domain.Exceptions;
using Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;
using Xunit;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;

public class UrlTests
{
    [Fact]
    public void Constructor_WithValidUrl_CreatesUrl()
    {
        var url = new Url("https://example.com");

        Assert.Equal("https://example.com", url.Value);
    }

    [Fact]
    public void Constructor_WithInvalidUrl_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => new Url("not-a-url"));

        Assert.Equal("Invalid URL format.", exception.Message);
    }

    [Fact]
    public void Constructor_WithNullValue_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Url(null!));
    }

    [Fact]
    public void Equals_TwoUrlsWithSameValue_ReturnsTrue()
    {
        var url1 = new Url("https://example.com");
        var url2 = new Url("https://example.com");

        Assert.Equal(url1, url2);
    }

    [Fact]
    public void Equals_TwoUrlsWithDifferentValues_ReturnsFalse()
    {
        var url1 = new Url("https://example.com");
        var url2 = new Url("https://other.com");

        Assert.NotEqual(url1, url2);
    }
}
