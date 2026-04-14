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
        public TenantId(Guid value) : base(value)
        {
            if (value == Guid.Empty)
                throw new DomainException("TenantId cannot be empty.");
        }

        public static TenantId New() => new(Guid.NewGuid());
    }
}
