using Pokok.BuildingBlocks.Domain.Exceptions;
using Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;
using Xunit;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;

public class DisplayNameTests
{
    [Fact]
    public void Constructor_WithValidValue_CreatesDisplayName()
    {
        var displayName = new DisplayName("My Display Name");

        Assert.Equal("My Display Name", displayName.Value);
    }

    [Fact]
    public void Constructor_WithEmptyValue_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => new DisplayName(""));

        Assert.Equal("Display name cannot be empty.", exception.Message);
    }

    [Fact]
    public void Constructor_WithWhitespaceValue_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => new DisplayName("   "));

        Assert.Equal("Display name cannot be empty.", exception.Message);
    }

    [Fact]
    public void Equals_TwoDisplayNamesWithSameValue_ReturnsTrue()
    {
        var name1 = new DisplayName("Admin");
        var name2 = new DisplayName("Admin");

        Assert.Equal(name1, name2);
    }

    [Fact]
    public void Equals_TwoDisplayNamesWithDifferentValues_ReturnsFalse()
    {
        var name1 = new DisplayName("Admin");
        var name2 = new DisplayName("User");

        Assert.NotEqual(name1, name2);
    }

    [Fact]
    public void ToString_WithValidValue_ReturnsValue()
    {
        var displayName = new DisplayName("My Name");

        Assert.Equal("My Name", displayName.ToString());
    }
}
