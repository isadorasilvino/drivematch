using DriveMatch.Domain.Exceptions;

namespace DriveMatch.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency = "BRL")
    {
        if (amount < 0)
            throw new DomainException("O valor monetário não pode ser negativo.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("A moeda deve ser informada.");

        Amount = decimal.Round(amount, 2, MidpointRounding.ToEven);
        Currency = currency.Trim().ToUpperInvariant();
    }
}
