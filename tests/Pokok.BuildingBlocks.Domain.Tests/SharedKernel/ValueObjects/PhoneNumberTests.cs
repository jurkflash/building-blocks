using Pokok.BuildingBlocks.Domain.Exceptions;
using Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;
using Xunit;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;

public class PhoneNumberTests
{
    [Fact]
    public void Constructor_WithValidPhoneNumber_CreatesPhoneNumber()
    {
        var phone = new PhoneNumber("+60123456789");

        Assert.Equal("+60123456789", phone.Value);
    }

    [Fact]
    public void Constructor_WithInvalidPhoneNumber_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => new PhoneNumber("abc"));

        Assert.Equal("Invalid phone number format.", exception.Message);
    }

    [Fact]
    public void Constructor_WithNullValue_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PhoneNumber(null!));
    }

    [Fact]
    public void Equals_TwoPhonesWithSameValue_ReturnsTrue()
    {
        var phone1 = new PhoneNumber("+60123456789");
        var phone2 = new PhoneNumber("+60123456789");

        Assert.Equal(phone1, phone2);
    }

    [Fact]
    public void Equals_TwoPhonesWithDifferentValues_ReturnsFalse()
    {
        var phone1 = new PhoneNumber("+60123456789");
        var phone2 = new PhoneNumber("+60198765432");

        Assert.NotEqual(phone1, phone2);
    }
}
