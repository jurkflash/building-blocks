namespace Pokok.BuildingBlocks.Domain.ValueObject
{
    public abstract class SingleValueObject<T> : ValueObject
    {
        public T Value { get; }

        protected SingleValueObject(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), $"{GetType().Name} cannot be null");

            Value = value;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value?.ToString() ?? string.Empty;
    }
}
