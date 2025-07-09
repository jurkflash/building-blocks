using Pokok.BuildingBlocks.Domain.Abstractions;
using System.Text.RegularExpressions;

namespace Pokok.BuildingBlocks.Domain.ValueObjects
{
    public sealed class PhoneNumber : SingleValueObject<string>
    {
        private static readonly Regex PhoneRegex =
            new(@"^\+?[0-9\s\-()]{7,20}$", RegexOptions.Compiled);

        public PhoneNumber(string value) : base(value)
        {
            if (!PhoneRegex.IsMatch(value))
                throw new ArgumentException("Invalid phone number format.", nameof(value));
        }
    }
}
