namespace Pokok.BuildingBlocks.Domain.ValueObjects
{
    public sealed class Versioned<T> : ValueObject
    {
        public T Value { get; }
        public int Version { get; }

        public Versioned(T value, int version)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Version = version;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
            yield return Version;
        }

        public override string ToString() => $"{Value} (v{Version})";
    }
}
