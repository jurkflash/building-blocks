using Pokok.BuildingBlocks.Domain.Abstractions;
using Xunit;

namespace Pokok.BuildingBlocks.Domain.Abstractions;

internal sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}

public class ValueObjectTests
{
    [Fact]
    public void Equals_TwoValueObjectsWithSameComponents_ReturnsTrue()
    {
        var money1 = new Money(10m, "USD");
        var money2 = new Money(10m, "USD");

        Assert.True(money1.Equals(money2));
    }

    [Fact]
    public void Equals_TwoValueObjectsWithDifferentComponents_ReturnsFalse()
    {
        var money1 = new Money(10m, "USD");
        var money2 = new Money(20m, "USD");

        Assert.False(money1.Equals(money2));
    }

    [Fact]
    public void Equals_ValueObjectComparedToNull_ReturnsFalse()
    {
        var money = new Money(10m, "USD");

        Assert.False(money.Equals(null));
    }

    [Fact]
    public void GetHashCode_TwoValueObjectsWithSameComponents_ReturnsSameHash()
    {
        var money1 = new Money(10m, "USD");
        var money2 = new Money(10m, "USD");

        Assert.Equal(money1.GetHashCode(), money2.GetHashCode());
    }

    [Fact]
    public void EqualityOperator_TwoValueObjectsWithSameComponents_ReturnsTrue()
    {
        var money1 = new Money(10m, "USD");
        var money2 = new Money(10m, "USD");

        Assert.True(money1 == money2);
    }

    [Fact]
    public void InequalityOperator_TwoValueObjectsWithDifferentComponents_ReturnsTrue()
    {
        var money1 = new Money(10m, "USD");
        var money2 = new Money(20m, "USD");

        Assert.True(money1 != money2);
    }
}
