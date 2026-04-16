using Pokok.BuildingBlocks.Domain.Abstractions;

namespace Pokok.BuildingBlocks.MultiTenancy
{
    /// <summary>
    /// Immutable value object representing a tenant identifier.
    /// Throws <see cref="ArgumentException"/> if the value is null or empty.
    /// </summary>
    public sealed class TenantId : ValueObject
    {
        /// <summary>
        /// Gets the underlying string value of the tenant identifier.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="TenantId"/> with the specified value.
        /// </summary>
        /// <param name="value">The tenant identifier string. Must not be null or whitespace.</param>
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
