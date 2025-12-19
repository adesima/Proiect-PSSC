using System.Text.Json.Serialization;

namespace Domain.Invoicing.Models;

public record OrderPlacedEvent
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public required BillingAddress BillingAddress { get; init; }
    public IReadOnlyCollection<OrderLine> Lines { get; init; } = Array.Empty<OrderLine>();
}

public record OrderLine
{
    public string ProductCode { get; init; }
    public int Quantity { get; init; }
    public Money UnitPrice { get; init; }

    [JsonConstructor]
    public OrderLine(string productCode, int quantity, Money unitPrice)
    {
        ProductCode = productCode;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
