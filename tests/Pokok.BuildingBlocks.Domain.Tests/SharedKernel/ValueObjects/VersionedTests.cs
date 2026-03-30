using Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;
using Xunit;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;

public class VersionedTests
{
    [Fact]
    public void Constructor_WithValidValueAndVersion_CreatesVersioned()
    {
        var versioned = new Versioned<string>("content", 1);

        Assert.Equal("content", versioned.Value);
        Assert.Equal(1, versioned.Version);
    }

    [Fact]
    public void Constructor_WithNullValue_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Versioned<string>(null!, 1));
    }

    [Fact]
    public void Equals_TwoVersionedWithSameValueAndVersion_ReturnsTrue()
    {
        var v1 = new Versioned<string>("data", 2);
        var v2 = new Versioned<string>("data", 2);

        Assert.Equal(v1, v2);
    }

    [Fact]
    public void Equals_TwoVersionedWithDifferentVersions_ReturnsFalse()
    {
        var v1 = new Versioned<string>("data", 1);
        var v2 = new Versioned<string>("data", 2);

        Assert.NotEqual(v1, v2);
    }

    [Fact]
    public void Equals_TwoVersionedWithDifferentValues_ReturnsFalse()
    {
        var v1 = new Versioned<string>("foo", 1);
        var v2 = new Versioned<string>("bar", 1);

        Assert.NotEqual(v1, v2);
    }

    [Fact]
    public void ToString_WithValueAndVersion_ReturnsFormattedString()
    {
        var versioned = new Versioned<string>("document", 3);

        Assert.Equal("document (v3)", versioned.ToString());
    }

    [Fact]
    public void GetHashCode_TwoVersionedWithSameComponents_ReturnsSameHash()
    {
        var v1 = new Versioned<string>("data", 1);
        var v2 = new Versioned<string>("data", 1);

        Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
    }
}
