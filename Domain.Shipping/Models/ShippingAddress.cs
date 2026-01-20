using System.Text.Json.Serialization;

namespace Domain.Shipping.Models
{
    public record ShippingAddress
    {
        [JsonPropertyOrder(1)]
        public string County { get; }
        [JsonPropertyOrder(2)]
        public string City { get; }
        [JsonPropertyOrder(3)]
        public string Street { get; }
        [JsonPropertyOrder(4)]
        public string PostalCode { get; }
        
        [JsonConstructor]
        public ShippingAddress(string county, string city, string street, string postalCode)
        {
            County = county;
            City = city;
            Street = street;
            PostalCode = postalCode;
        }

        public static ShippingAddress Create(string county, string city, string street, string postalCode)
        {
            if (string.IsNullOrWhiteSpace(county))
                throw new ArgumentException("Street is required.", nameof(county));

            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City is required.", nameof(city));

            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("Street is required.", nameof(street));


            if (string.IsNullOrWhiteSpace(postalCode))
                throw new ArgumentException("Postal code is required.", nameof(postalCode));

            return new ShippingAddress(county.Trim(), city.Trim(), street.Trim(), postalCode.Trim());
        }
        
        public override string ToString() => $"{County}, {City}, {Street}, {PostalCode}";
    }
}