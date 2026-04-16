using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Exceptions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    /// <summary>
    /// Immutable value object representing a full postal address.
    /// All fields (Street, City, State, PostalCode, Country) are required.
    /// Throws <see cref="DomainException"/> if any field is empty.
    /// </summary>
    public sealed class Address : ValueObject
    {
        /// <summary>Gets the street address.</summary>
        public string Street { get; }

        /// <summary>Gets the city name.</summary>
        public string City { get; }

        /// <summary>Gets the state or province.</summary>
        public string State { get; }

        /// <summary>Gets the postal or ZIP code.</summary>
        public string PostalCode { get; }

        /// <summary>Gets the country name.</summary>
        public string Country { get; }

        /// <summary>
        /// Initializes a new <see cref="Address"/> with the specified components.
        /// </summary>
        /// <param name="street">The street address.</param>
        /// <param name="city">The city name.</param>
        /// <param name="state">The state or province.</param>
        /// <param name="postalCode">The postal or ZIP code.</param>
        /// <param name="country">The country name.</param>
        public Address(string street, string city, string state, string postalCode, string country)
        {
            Street = street;
            City = city;
            State = state;
            PostalCode = postalCode;
            Country = country;

            Validate();
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Street))
                throw new DomainException("Street is required.");
            if (string.IsNullOrWhiteSpace(City))
                throw new DomainException("City is required.");
            if (string.IsNullOrWhiteSpace(State))
                throw new DomainException("State is required.");
            if (string.IsNullOrWhiteSpace(PostalCode))
                throw new DomainException("Postal code is required.");
            if (string.IsNullOrWhiteSpace(Country))
                throw new DomainException("Country is required.");
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return State;
            yield return PostalCode;
            yield return Country;
        }
    }
}
