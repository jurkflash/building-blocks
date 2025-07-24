using Pokok.BuildingBlocks.Domain.Abstractions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.Enums
{
    public sealed class OutboxMessageType : ValueObject
    {
        public string Value { get; }

        private OutboxMessageType(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static OutboxMessageType Email => new("Email");

        public static OutboxMessageType Custom(string value) => new(value);

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static OutboxMessageType From(string value)
        {
            return new OutboxMessageType(value);
        }
    }
}
