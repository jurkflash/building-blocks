using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    /// <summary>
    /// Immutable value object representing a validated email address.
    /// Validates against the pattern <c>^[^@\s]+@[^@\s]+\.[^@\s]+$</c>.
    /// Throws <see cref="DomainException"/> if the format is invalid.
    /// </summary>
    public sealed class Email : SingleValueObject<string>
    {
        private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public Email(string value) : base(value)
        {
            Validate();
        }

        protected override void Validate()
        {
            if (!EmailRegex.IsMatch(Value))
                throw new DomainException("Invalid email format.");
        }
    }
}
