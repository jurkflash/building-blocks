namespace Pokok.BuildingBlocks.Domain.Abstractions
{
    /// <summary>
    /// Strongly-typed entity identifier that wraps a value of type <typeparamref name="T"/>.
    /// Provides value-based equality for entity IDs (e.g., <c>UserId</c>, <c>OrderId</c>).
    /// </summary>
    /// <typeparam name="T">The underlying identifier type (e.g., <see cref="Guid"/>, <see cref="string"/>).</typeparam>
    public abstract class EntityId<T> : SingleValueObject<T>
    {
        protected EntityId(T value) : base(value)
        {
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value?.ToString() ?? string.Empty;
    }
}
