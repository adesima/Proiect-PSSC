using System.Text.Json.Serialization;
using Domain.Shipping.Models; 

namespace Domain.Shipping.Models;

// Acesta este mesajul pe care îl primești tu de la Facturare (Input-ul tău)
public record InvoicePaidEvent
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    
    // Tu ai nevoie de adresa unde livrezi
    public required ShippingAddress ShippingAddress { get; init; }
    
    public IReadOnlyCollection<ShipmentLine> Lines { get; init; } = Array.Empty<ShipmentLine>();
}

// Linia de livrare - simplificată (fără preț)
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