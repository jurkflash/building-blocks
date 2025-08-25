using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    public sealed class Email : SingleValueObject<string>
    {
        private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public Email(string value) : base(value)
        {
        }

        protected override void Validate()
        {
            if (!EmailRegex.IsMatch(Value))
                throw new DomainException("Invalid email format.");
        }
    }
}
