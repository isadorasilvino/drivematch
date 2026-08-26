using DriveMatch.Domain.Exceptions;
using DriveMatch.Domain.ValueObjects;

namespace DriveMatch.UnitTests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Constructor_ShouldCreateMoney_WhenDataIsValid()
    {
        var money = new Money(100.50m, "BRL");

        Assert.Equal(100.50m, money.Amount);
        Assert.Equal("BRL", money.Currency);
    }

    [Fact]
    public void Constructor_ShouldNormalizeCurrency()
    {
        var money = new Money(100m, " brl ");

        Assert.Equal("BRL", money.Currency);
    }

    [Fact]
    public void Constructor_ShouldRoundAmountToTwoDecimalPlaces()
    {
        var money = new Money(10.555m);

        Assert.Equal(10.56m, money.Amount);
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenAmountIsNegative()
    {
        var exception = Assert.Throws<DomainException>(
            () => new Money(-1m));

        Assert.Equal(
            "O valor monetário não pode ser negativo.",
            exception.Message);
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenCurrencyIsEmpty()
    {
        var exception = Assert.Throws<DomainException>(
            () => new Money(10m, " "));

        Assert.Equal(
            "A moeda deve ser informada.",
            exception.Message);
    }

    [Fact]
    public void Constructor_ShouldUseBrlAsDefaultCurrency()
    {
        var money = new Money(100m);

        Assert.Equal("BRL", money.Currency);
    }

    [Fact]
    public void Money_ShouldUseValueEquality()
    {
        var first = new Money(100m, "BRL");
        var second = new Money(100m, "BRL");

        Assert.Equal(first, second);
    }
}
