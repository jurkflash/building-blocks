using Pokok.BuildingBlocks.Domain.Abstractions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    public sealed class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            if (amount < 0) throw new ArgumentException("Amount cannot be negative.");
            if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency is required.");

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
            if (Currency != other.Currency) throw new InvalidOperationException("Currency mismatch.");
            return new Money(Amount + other.Amount, Currency);
        }
    }
}
