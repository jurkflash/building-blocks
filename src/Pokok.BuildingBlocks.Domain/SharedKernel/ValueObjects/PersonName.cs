using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Exceptions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    /// <summary>
    /// Immutable value object representing a person's first and last name.
    /// Both fields are required. Throws <see cref="DomainException"/> if either is empty.
    /// </summary>
    public sealed class PersonName : ValueObject
    {
        /// <summary>Gets the first name.</summary>
        public string FirstName { get; }

        /// <summary>Gets the last name.</summary>
        public string LastName { get; }

        /// <summary>
        /// Initializes a new <see cref="PersonName"/> with the specified first and last name.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        public PersonName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;

            Validate();
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
                throw new DomainException("First name is required.");
            if (string.IsNullOrWhiteSpace(LastName))
                throw new DomainException("Last name is required.");
        }

        /// <summary>Gets the full name as "FirstName LastName".</summary>
        public string FullName => $"{FirstName} {LastName}";

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
        }
    }
}
