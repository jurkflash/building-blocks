using Pokok.BuildingBlocks.Domain.Abstractions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    public sealed class DisplayName : SingleValueObject<string>
    {
        public DisplayName(string value) : base(value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("DisplayName cannot be empty.", nameof(value));
        }
    }
}
