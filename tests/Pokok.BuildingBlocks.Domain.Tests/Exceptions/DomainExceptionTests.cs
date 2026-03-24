using Pokok.BuildingBlocks.Domain.Exceptions;
using Xunit;

namespace Pokok.BuildingBlocks.Domain.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        var exception = new DomainException("Something went wrong.");

        Assert.Equal("Something went wrong.", exception.Message);
    }

    [Fact]
    public void Constructor_IsException_InheritsFromException()
    {
        var exception = new DomainException("error");

        Assert.IsAssignableFrom<Exception>(exception);
    }
}
