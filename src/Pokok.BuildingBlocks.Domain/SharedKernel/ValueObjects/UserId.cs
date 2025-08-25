using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Exceptions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    public sealed class UserId : EntityId<Guid>
    {
        public UserId(Guid value) : base(value)
        {
            if (value == Guid.Empty)
                throw new DomainException("UserId cannot be empty");
        }

        public static UserId New() => new(Guid.NewGuid());
    }
}
