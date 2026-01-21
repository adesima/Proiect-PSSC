using System.Text.Json.Serialization;
using Domain.Shipping.Models; 

namespace Domain.Shipping.Models;

public record InvoicePaidEvent
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    
    public required ShippingAddress ShippingAddress { get; init; }
    
    public IReadOnlyCollection<ShipmentLine> Lines { get; init; } = Array.Empty<ShipmentLine>();
}

public record ShipmentLine
{
    public string ProductCode { get; init; }
    public int Quantity { get; init; }

    [JsonConstructor]
    public ShipmentLine(string productCode, int quantity)
    {
        ProductCode = productCode;
        Quantity = quantity;
    }
}