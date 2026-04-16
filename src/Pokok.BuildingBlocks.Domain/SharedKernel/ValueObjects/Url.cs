using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Exceptions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    /// <summary>
    /// Immutable value object representing a validated absolute URL.
    /// Throws <see cref="DomainException"/> if the value is not a well-formed absolute URI.
    /// </summary>
    public sealed class Url : SingleValueObject<string>
    {
        /// <summary>
        /// Initializes a new <see cref="Url"/> with the specified absolute URI string.
        /// </summary>
        /// <param name="value">The absolute URL string.</param>
        public Url(string value) : base(value)
        {
            Validate();
        }

        protected override void Validate()
        {
            if (!Uri.IsWellFormedUriString(Value, UriKind.Absolute))
                throw new DomainException("Invalid URL format.");
        }
    }
}
