using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Exceptions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.Enums
{
    /// <summary>
    /// Enumeration-style value object representing the type of an outbox message (e.g., Email, Custom).
    /// Throws <see cref="DomainException"/> if the value is null or empty.
    /// </summary>
    public sealed class OutboxMessageType : ValueObject
    {
        public string Value { get; }

        private OutboxMessageType(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException($"{nameof(OutboxMessageType)} cannot be null or empty.");

            Value = value;
        }

        public static OutboxMessageType Email => new("Email");

        public static OutboxMessageType Custom(string value) => new(value);

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static OutboxMessageType From(string value) => new(value);
    }
}
