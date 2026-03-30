using Pokok.BuildingBlocks.Domain.Exceptions;
using Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;
using Xunit;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Constructor_WithValidAmountAndCurrency_CreatesMoney()
    {
        var money = new Money(100m, "USD");

        Assert.Equal(100m, money.Amount);
        Assert.Equal("USD", money.Currency);
    }

    [Fact]
    public void Constructor_WithLowercaseCurrency_NormalizesToUppercase()
    {
        var money = new Money(50m, "usd");

        Assert.Equal("USD", money.Currency);
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => new Money(-1m, "USD"));

        Assert.Equal("Amount cannot be negative.", exception.Message);
    }

    [Fact]
    public void Constructor_WithEmptyCurrency_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => new Money(10m, ""));

        Assert.Equal("Currency is required.", exception.Message);
    }

    [Fact]
    public void Constructor_WithWhitespaceCurrency_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => new Money(10m, "   "));

        Assert.Equal("Currency is required.", exception.Message);
    }

    [Fact]
    public void Add_TwoMoniesWithSameCurrency_ReturnsSummedMoney()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "USD");

        var result = money1.Add(money2);

        Assert.Equal(150m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Add_TwoMoniesWithDifferentCurrencies_ThrowsDomainException()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "EUR");

        var exception = Assert.Throws<DomainException>(() => money1.Add(money2));

        Assert.Equal("Currency mismatch.", exception.Message);
    }

    [Fact]
    public void Equals_TwoMoniesWithSameAmountAndCurrency_ReturnsTrue()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(100m, "USD");

        Assert.Equal(money1, money2);
    }

    [Fact]
    public void Equals_TwoMoniesWithDifferentAmounts_ReturnsFalse()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(200m, "USD");

        Assert.NotEqual(money1, money2);
    }
}
