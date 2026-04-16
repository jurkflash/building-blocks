using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Exceptions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    /// <summary>
    /// Strongly-typed user identifier. Wraps a <see cref="Guid"/> value.
    /// Throws <see cref="DomainException"/> if the GUID is <see cref="Guid.Empty"/>.
    /// </summary>
    public sealed class UserId : EntityId<Guid>
    {
        /// <summary>
        /// Initializes a new <see cref="UserId"/> with the specified GUID.
        /// </summary>
        /// <param name="value">The user identifier value.</param>
        public UserId(Guid value) : base(value)
        {
            if (value == Guid.Empty)
                throw new DomainException("UserId cannot be empty");
        }

        /// <summary>
        /// Creates a new <see cref="UserId"/> with a randomly generated GUID.
        /// </summary>
        /// <returns>A new unique <see cref="UserId"/>.</returns>
        public static UserId New() => new(Guid.NewGuid());
    }
}
