using Pokok.BuildingBlocks.Domain.Exceptions;
using Pokok.BuildingBlocks.Domain.SharedKernel.Enums;
using Xunit;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.Enums;

public class OutboxMessageTypeTests
{
    [Fact]
    public void Email_WhenAccessed_ReturnsEmailType()
    {
        var type = OutboxMessageType.Email;

        Assert.Equal("Email", type.Value);
    }

    [Fact]
    public void Custom_WithValidValue_CreatesCustomType()
    {
        var type = OutboxMessageType.Custom("Sms");

        Assert.Equal("Sms", type.Value);
    }

    [Fact]
    public void From_WithValidValue_CreatesType()
    {
        var type = OutboxMessageType.From("Notification");

        Assert.Equal("Notification", type.Value);
    }

    [Fact]
    public void From_WithEmptyValue_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => OutboxMessageType.From(""));
    }

    [Fact]
    public void From_WithWhitespaceValue_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => OutboxMessageType.From("   "));
    }

    [Fact]
    public void ToString_WithValidValue_ReturnsValue()
    {
        var type = OutboxMessageType.Email;

        Assert.Equal("Email", type.ToString());
    }

    [Fact]
    public void Equals_TwoTypesWithSameValue_ReturnsTrue()
    {
        var type1 = OutboxMessageType.From("Push");
        var type2 = OutboxMessageType.From("Push");

        Assert.Equal(type1, type2);
    }

    [Fact]
    public void Equals_TwoTypesWithDifferentValues_ReturnsFalse()
    {
        var type1 = OutboxMessageType.Email;
        var type2 = OutboxMessageType.Custom("Sms");

        Assert.NotEqual(type1, type2);
    }
}
