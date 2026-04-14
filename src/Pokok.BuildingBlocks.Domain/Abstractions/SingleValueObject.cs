namespace Pokok.BuildingBlocks.Domain.Abstractions
{
    /// <summary>
    /// A value object that wraps a single primitive value of type <typeparamref name="T"/>.
    /// Throws <see cref="ArgumentNullException"/> if the value is <c>null</c>.
    /// Override <see cref="Validate(T)"/> to add domain-specific validation.
    /// </summary>
    /// <typeparam name="T">The type of the wrapped value.</typeparam>
    public abstract class SingleValueObject<T> : ValueObject
    {
        public T Value { get; }

        protected SingleValueObject(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), $"{GetType().Name} cannot be null");

            Validate(value);

            Value = value;
        }

        protected virtual void Validate(T value)
        {
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value?.ToString() ?? string.Empty;
    }
}
