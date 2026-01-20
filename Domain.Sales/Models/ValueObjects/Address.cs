using System;
using Domain.Sales.Exceptions;

namespace Domain.Sales.Models.ValueObjects
{
    public record Address
    {
        public string County { get; } 
        public string City { get; }
        public string Street { get; }
        public string PostalCode { get; }

        public Address(string county, string city, string street, string postalCode)
        {
            if (string.IsNullOrWhiteSpace(county)) throw new InvalidAddressException("County cannot be empty");
            if (string.IsNullOrWhiteSpace(city)) throw new InvalidAddressException("City cannot be empty");
            if (string.IsNullOrWhiteSpace(street)) throw new InvalidAddressException("Street cannot be empty");
            if (string.IsNullOrWhiteSpace(postalCode)) throw new InvalidAddressException("PostalCode cannot be empty");
            
            County = county;
            City = city;
            Street = street;
            PostalCode = postalCode;
        }

        public override string ToString() => $" {County}, {City}, {Street}, {PostalCode}";
    }
}