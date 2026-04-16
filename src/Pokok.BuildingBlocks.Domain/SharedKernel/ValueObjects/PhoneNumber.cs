using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    /// <summary>
    /// Immutable value object representing a validated phone number.
    /// Validates against the pattern <c>^\+?[0-9\s\-()]{7,20}$</c>.
    /// Throws <see cref="DomainException"/> if the format is invalid.
    /// </summary>
    public sealed class PhoneNumber : SingleValueObject<string>
    {
        private static readonly Regex PhoneRegex =
            new(@"^\+?[0-9\s\-()]{7,20}$", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new <see cref="PhoneNumber"/> with the specified value.
        /// </summary>
        /// <param name="value">The phone number string.</param>
        public PhoneNumber(string value) : base(value)
        {
            Validate();
        }

        protected override void Validate()
        {
            if (!PhoneRegex.IsMatch(Value))
                throw new DomainException("Invalid phone number format.");
        }
    }
}
