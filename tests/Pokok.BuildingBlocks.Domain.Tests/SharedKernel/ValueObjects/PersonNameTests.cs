using Pokok.BuildingBlocks.Domain.Exceptions;
using Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;
using Xunit;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;

public class PersonNameTests
{
    [Fact]
    public void Constructor_WithValidFirstAndLastName_CreatesPersonName()
    {
        var name = new PersonName("John", "Doe");

        Assert.Equal("John", name.FirstName);
        Assert.Equal("Doe", name.LastName);
    }

    [Fact]
    public void FullName_WithFirstAndLastName_ReturnsCombinedName()
    {
        var name = new PersonName("John", "Doe");

        Assert.Equal("John Doe", name.FullName);
    }

    [Fact]
    public void Constructor_WithEmptyFirstName_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => new PersonName("", "Doe"));

        Assert.Equal("First name is required.", exception.Message);
    }

    [Fact]
    public void Constructor_WithEmptyLastName_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => new PersonName("John", ""));

        Assert.Equal("Last name is required.", exception.Message);
    }

    [Fact]
    public void Equals_TwoPersonNamesWithSameValues_ReturnsTrue()
    {
        var name1 = new PersonName("John", "Doe");
        var name2 = new PersonName("John", "Doe");

        Assert.Equal(name1, name2);
    }

    [Fact]
    public void Equals_TwoPersonNamesWithDifferentLastNames_ReturnsFalse()
    {
        var name1 = new PersonName("John", "Doe");
        var name2 = new PersonName("John", "Smith");

        Assert.NotEqual(name1, name2);
    }
}
