using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Exceptions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    /// <summary>
    /// Immutable value object representing a non-empty display name.
    /// Throws <see cref="DomainException"/> if the value is null or whitespace.
    /// </summary>
    public sealed class DisplayName : SingleValueObject<string>
    {
        public DisplayName(string value) : base(value)
        {
            Validate();
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Value))
                throw new DomainException("Display name cannot be empty.");
        }
    }
}
