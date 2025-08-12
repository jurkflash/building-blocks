using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Exceptions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    public sealed class DisplayName : SingleValueObject<string>
    {
        public DisplayName(string value) : base(value)
        {
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Value))
                throw new DomainException("Display name cannot be empty.");
        }
    }
}
