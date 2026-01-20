using Domain.Sales.Exceptions;

namespace Domain.Sales.Models.ValueObjects
{
    public record ShippingAddress
    {
        public string Street { get; }
        public string City { get; }
        public string PostalCode { get; }

        public ShippingAddress(string street, string city, string postalCode)
        {
            if (string.IsNullOrEmpty(street)) throw new InvalidAddressException("Street cannot be empty.");
            if (string.IsNullOrEmpty(city)) throw new InvalidAddressException("City cannot be empty.");
            if (string.IsNullOrEmpty(postalCode)) throw new InvalidAddressException("Postal code cannot be empty.");

            Street = street;
            City = city;
            PostalCode = postalCode;
        }

        public override string ToString() => $"{Street}, {City}, {PostalCode}";
    }
}