namespace Pokok.BuildingBlocks.Domain.Abstractions
{
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
