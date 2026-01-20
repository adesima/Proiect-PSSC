using System;

namespace Domain.Sales.Models.ValueObjects
{
    public record Address
    {
        public string City { get; }
        public string Street { get; }
        public string PostalCode { get; }

        public Address(string city, string street, string postalCode)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                throw new ArgumentException("City cannot be empty", nameof(city));
            }

            if (string.IsNullOrWhiteSpace(street))
            {
                throw new ArgumentException("Street cannot be empty", nameof(street));
            }

            if (string.IsNullOrWhiteSpace(postalCode))
            {
                throw new ArgumentException("PostalCode cannot be empty", nameof(postalCode));
            }

            City = city;
            Street = street;
            PostalCode = postalCode;
        }

        // Metodă ajutătoare pentru afișare
        public override string ToString()
        {
            return $"{Street}, {City}, {PostalCode}";
        }
    }
}