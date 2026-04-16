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
        /// <summary>
        /// Gets the string value of this outbox message type.
        /// </summary>
        public string Value { get; }

        private OutboxMessageType(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException($"{nameof(OutboxMessageType)} cannot be null or empty.");

            Value = value;
        }

        /// <summary>
        /// Gets the predefined email outbox message type.
        /// </summary>
        public static OutboxMessageType Email => new("Email");

        /// <summary>
        /// Creates a custom outbox message type with the specified value.
        /// </summary>
        /// <param name="value">The custom message type value.</param>
        /// <returns>A new <see cref="OutboxMessageType"/> instance.</returns>
        public static OutboxMessageType Custom(string value) => new(value);

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        /// <summary>
        /// Creates an <see cref="OutboxMessageType"/> from the specified string value.
        /// </summary>
        /// <param name="value">The message type value.</param>
        /// <returns>A new <see cref="OutboxMessageType"/> instance.</returns>
        public static OutboxMessageType From(string value) => new(value);
    }
}
