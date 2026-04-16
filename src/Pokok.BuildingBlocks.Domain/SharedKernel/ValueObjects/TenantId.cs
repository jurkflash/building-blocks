using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Exceptions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    /// <summary>
    /// Strongly-typed tenant identifier. Wraps a <see cref="Guid"/> value.
    /// Throws <see cref="DomainException"/> if the GUID is <see cref="Guid.Empty"/>.
    /// </summary>
    public sealed class TenantId : EntityId<Guid>
    {
        /// <summary>
        /// Initializes a new <see cref="TenantId"/> with the specified GUID.
        /// </summary>
        /// <param name="value">The tenant identifier value.</param>
        public TenantId(Guid value) : base(value)
        {
            if (value == Guid.Empty)
                throw new DomainException("TenantId cannot be empty.");
        }

        /// <summary>
        /// Creates a new <see cref="TenantId"/> with a randomly generated GUID.
        /// </summary>
        /// <returns>A new unique <see cref="TenantId"/>.</returns>
        public static TenantId New() => new(Guid.NewGuid());
    }
}
