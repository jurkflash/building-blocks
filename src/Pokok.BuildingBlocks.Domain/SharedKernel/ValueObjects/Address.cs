using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Exceptions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    public sealed class Address : ValueObject
    {
        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string PostalCode { get; }
        public string Country { get; }

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
