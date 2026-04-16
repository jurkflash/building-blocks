using Pokok.BuildingBlocks.Domain.Abstractions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    /// <summary>
    /// Immutable value object that wraps a value of type <typeparamref name="T"/> with a version number.
    /// Useful for optimistic concurrency or tracking entity revisions.
    /// </summary>
    /// <typeparam name="T">The type of the wrapped value.</typeparam>
    public sealed class Versioned<T> : ValueObject
    {
        /// <summary>Gets the wrapped value.</summary>
        public T Value { get; }

        /// <summary>Gets the version number.</summary>
        public int Version { get; }

        /// <summary>
        /// Initializes a new <see cref="Versioned{T}"/> with the specified value and version.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="version">The version number.</param>
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
