using Pokok.BuildingBlocks.Domain.Abstractions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    public sealed class Url : SingleValueObject<string>
    {
        public Url(string value) : base(value)
        {
            if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
                throw new ArgumentException("Invalid URL format.", nameof(value));
        }
    }
}
