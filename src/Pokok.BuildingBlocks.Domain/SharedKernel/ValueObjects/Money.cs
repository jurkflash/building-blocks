using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Exceptions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    /// <summary>
    /// Immutable value object representing a monetary amount with currency.
    /// Amount must be non-negative and currency is required.
    /// Throws <see cref="DomainException"/> on negative amount, missing currency, or currency mismatch during addition.
    /// </summary>
    public sealed class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            if (amount < 0) throw new DomainException("Amount cannot be negative.");
            if (string.IsNullOrWhiteSpace(currency)) throw new DomainException("Currency is required.");

            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency) throw new DomainException("Currency mismatch.");
            return new Money(Amount + other.Amount, Currency);
        }
    }
}
