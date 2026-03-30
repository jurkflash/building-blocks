using Pokok.BuildingBlocks.Cqrs.Validation;
using Xunit;

namespace Pokok.BuildingBlocks.Cqrs.Validation;

public class ValidationExceptionTests
{
    [Fact]
    public void Constructor_WithErrors_SetsErrorsList()
    {
        var errors = new[] { "Field is required.", "Value is invalid." };

        var exception = new ValidationException(errors);

        Assert.Equal(2, exception.Errors.Count);
        Assert.Contains("Field is required.", exception.Errors);
        Assert.Contains("Value is invalid.", exception.Errors);
    }

    [Fact]
    public void Constructor_WithEmptyErrors_SetsEmptyList()
    {
        var exception = new ValidationException(Array.Empty<string>());

        Assert.Empty(exception.Errors);
    }

    [Fact]
    public void Message_WhenCreated_ReturnsValidationFailedMessage()
    {
        var exception = new ValidationException(new[] { "error" });

        Assert.Equal("Validation failed.", exception.Message);
    }

    [Fact]
    public void Errors_IsReadOnly_CannotBeModified()
    {
        var exception = new ValidationException(new[] { "error" });

        Assert.IsAssignableFrom<IReadOnlyList<string>>(exception.Errors);
    }

    [Fact]
    public void Constructor_IsException_InheritsFromException()
    {
        var exception = new ValidationException(new[] { "error" });

        Assert.IsAssignableFrom<Exception>(exception);
    }
}
