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
        /// <summary>Gets the monetary amount.</summary>
        public decimal Amount { get; }

        /// <summary>Gets the ISO currency code (uppercase).</summary>
        public string Currency { get; }

        /// <summary>
        /// Initializes a new <see cref="Money"/> with the specified amount and currency.
        /// </summary>
        /// <param name="amount">The non-negative monetary amount.</param>
        /// <param name="currency">The currency code.</param>
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

        /// <summary>
        /// Adds another <see cref="Money"/> value with the same currency to this instance.
        /// </summary>
        /// <param name="other">The money value to add.</param>
        /// <returns>A new <see cref="Money"/> representing the sum.</returns>
        public Money Add(Money other)
        {
            if (Currency != other.Currency) throw new DomainException("Currency mismatch.");
            return new Money(Amount + other.Amount, Currency);
        }
    }
}
