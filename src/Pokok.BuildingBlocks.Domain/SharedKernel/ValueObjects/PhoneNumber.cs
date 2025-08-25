using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    public sealed class PhoneNumber : SingleValueObject<string>
    {
        private static readonly Regex PhoneRegex =
            new(@"^\+?[0-9\s\-()]{7,20}$", RegexOptions.Compiled);

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
