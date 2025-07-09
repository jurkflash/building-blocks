using Pokok.BuildingBlocks.Domain.Abstractions;
using System.Text.RegularExpressions;

namespace Pokok.BuildingBlocks.Domain.ValueObjects
{
    public sealed class Email : SingleValueObject<string>
    {
        private static readonly Regex EmailRegex =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public Email(string value) : base(value)
        {
            if (!EmailRegex.IsMatch(value))
                throw new ArgumentException("Invalid email format.", nameof(value));
        }
    }
}
