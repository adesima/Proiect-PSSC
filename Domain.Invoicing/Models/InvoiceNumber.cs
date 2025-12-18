namespace Domain.Invoicing.Models;

public record InvoiceNumber
{
    public string Value { get; }

    private InvoiceNumber(string value)
    {
        Value = value;
    }

    public static InvoiceNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Invoice number is required.", nameof(value));

        // Simple validation: invoice number must be at least 3 characters long
        if (value.Length < 3)
            throw new ArgumentException("Invoice number is too short.", nameof(value));

        return new InvoiceNumber(value.Trim().ToUpperInvariant());
    }

    public override string ToString() => Value;
}
