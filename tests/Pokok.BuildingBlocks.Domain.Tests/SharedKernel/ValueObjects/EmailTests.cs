using Pokok.BuildingBlocks.Domain.Exceptions;
using Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;
using Xunit;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Constructor_WithValidEmail_CreatesEmail()
    {
        var email = new Email("user@example.com");

        Assert.Equal("user@example.com", email.Value);
    }

    [Fact]
    public void Constructor_WithInvalidEmailFormat_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => new Email("not-an-email"));

        Assert.Equal("Invalid email format.", exception.Message);
    }

    [Fact]
    public void Constructor_WithMissingAtSign_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new Email("userexample.com"));
    }

    [Fact]
    public void Constructor_WithNullValue_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Email(null!));
    }

    [Fact]
    public void Equals_TwoEmailsWithSameValue_ReturnsTrue()
    {
        var email1 = new Email("user@example.com");
        var email2 = new Email("user@example.com");

        Assert.Equal(email1, email2);
    }

    [Fact]
    public void Equals_TwoEmailsWithDifferentValues_ReturnsFalse()
    {
        var email1 = new Email("user@example.com");
        var email2 = new Email("other@example.com");

        Assert.NotEqual(email1, email2);
    }

    [Fact]
    public void ToString_WithValidEmail_ReturnsEmailString()
    {
        var email = new Email("user@example.com");

        Assert.Equal("user@example.com", email.ToString());
    }
}
