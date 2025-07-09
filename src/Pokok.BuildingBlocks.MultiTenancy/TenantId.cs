using Pokok.BuildingBlocks.Domain.Abstractions;

namespace Pokok.BuildingBlocks.MultiTenancy
{
    public sealed class TenantId : ValueObject
    {
        public string Value { get; }

        public TenantId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Tenant ID cannot be null or empty.", nameof(value));

            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
