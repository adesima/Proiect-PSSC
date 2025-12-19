using System.Text.Json.Serialization;

namespace Domain.Invoicing.Models;

public record BillingAddress
{
    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }

    [JsonConstructor]
    private BillingAddress(string street, string city, string postalCode)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
    }

    public static BillingAddress Create(string street, string city, string postalCode)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street is required.", nameof(street));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required.", nameof(city));

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code is required.", nameof(postalCode));

        return new BillingAddress(street.Trim(), city.Trim(), postalCode.Trim());
    }

    public override string ToString() => $"{Street}, {City}, {PostalCode}";
}
