using System.Text.Json.Serialization;

namespace Domain.Shipping.Models; // Modificat din Domain.Invoicing.Models

public record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    [JsonConstructor]
    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency is required.", nameof(currency));
        }

        return new Money(decimal.Round(amount, 2), currency.ToUpperInvariant());
    }

    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new InvalidOperationException("Cannot add amounts with different currencies.");
        }

        return Create(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator *(Money left, int quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));
        }

        return Create(left.Amount * quantity, left.Currency);
    }

    public override string ToString() => $"{Amount:0.00} {Currency}";
}