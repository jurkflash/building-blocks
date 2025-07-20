using Pokok.BuildingBlocks.Domain.Abstractions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    public sealed class UserId : SingleValueObject<Guid>
    {
        public UserId(Guid value) : base(value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(value));
        }

        public static UserId New() => new(Guid.NewGuid());
    }
}
