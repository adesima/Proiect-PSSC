using System.Text.Json.Serialization;

namespace Domain.Shipping.Models
{
    public record ShippingAddress
    {
        public string Street { get; }
        public string City { get; }
        public string PostalCode { get; }
        public string County { get; }

        [JsonConstructor]
        private ShippingAddress(string street, string city, string postalCode, string county)
        {
            Street = street;
            City = city;
            PostalCode = postalCode;
            County = county;
        }

        public static ShippingAddress Create(string street, string city, string postalCode, string county)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("Street is required.", nameof(street));

            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City is required.", nameof(city));

            if (string.IsNullOrWhiteSpace(postalCode))
                throw new ArgumentException("Postal code is required.", nameof(postalCode));

            if (string.IsNullOrWhiteSpace(county))
                throw new ArgumentException("County is required.", nameof(county));

            return new ShippingAddress(street, city, postalCode, county);
        }

        public override string ToString() => $"{Street}, {City}, {County}, {PostalCode}";
    }
}