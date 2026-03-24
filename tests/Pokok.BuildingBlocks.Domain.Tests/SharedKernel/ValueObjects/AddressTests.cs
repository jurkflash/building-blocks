using Pokok.BuildingBlocks.Domain.Exceptions;
using Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;
using Xunit;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;

public class AddressTests
{
    [Fact]
    public void Constructor_WithAllValidFields_CreatesAddress()
    {
        var address = new Address("123 Main St", "Springfield", "IL", "62701", "US");

        Assert.Equal("123 Main St", address.Street);
        Assert.Equal("Springfield", address.City);
        Assert.Equal("IL", address.State);
        Assert.Equal("62701", address.PostalCode);
        Assert.Equal("US", address.Country);
    }

    [Fact]
    public void Constructor_WithEmptyStreet_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(
            () => new Address("", "Springfield", "IL", "62701", "US"));

        Assert.Equal("Street is required.", exception.Message);
    }

    [Fact]
    public void Constructor_WithEmptyCity_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(
            () => new Address("123 Main St", "", "IL", "62701", "US"));

        Assert.Equal("City is required.", exception.Message);
    }

    [Fact]
    public void Constructor_WithEmptyState_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(
            () => new Address("123 Main St", "Springfield", "", "62701", "US"));

        Assert.Equal("State is required.", exception.Message);
    }

    [Fact]
    public void Constructor_WithEmptyPostalCode_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(
            () => new Address("123 Main St", "Springfield", "IL", "", "US"));

        Assert.Equal("Postal code is required.", exception.Message);
    }

    [Fact]
    public void Constructor_WithEmptyCountry_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(
            () => new Address("123 Main St", "Springfield", "IL", "62701", ""));

        Assert.Equal("Country is required.", exception.Message);
    }

    [Fact]
    public void Equals_TwoAddressesWithSameFields_ReturnsTrue()
    {
        var address1 = new Address("123 Main St", "Springfield", "IL", "62701", "US");
        var address2 = new Address("123 Main St", "Springfield", "IL", "62701", "US");

        Assert.Equal(address1, address2);
    }

    [Fact]
    public void Equals_TwoAddressesWithDifferentStreets_ReturnsFalse()
    {
        var address1 = new Address("123 Main St", "Springfield", "IL", "62701", "US");
        var address2 = new Address("456 Oak Ave", "Springfield", "IL", "62701", "US");

        Assert.NotEqual(address1, address2);
    }
}
