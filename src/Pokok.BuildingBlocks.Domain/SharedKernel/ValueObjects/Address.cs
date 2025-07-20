using Pokok.BuildingBlocks.Domain.Abstractions;

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
            Street = string.IsNullOrWhiteSpace(street) ? throw new ArgumentException("Street is required.") : street;
            City = string.IsNullOrWhiteSpace(city) ? throw new ArgumentException("City is required.") : city;
            State = string.IsNullOrWhiteSpace(state) ? throw new ArgumentException("State is required.") : state;
            PostalCode = string.IsNullOrWhiteSpace(postalCode) ? throw new ArgumentException("Postal code is required.") : postalCode;
            Country = string.IsNullOrWhiteSpace(country) ? throw new ArgumentException("Country is required.") : country;
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
