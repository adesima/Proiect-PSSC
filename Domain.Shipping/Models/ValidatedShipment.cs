using Domain.Shipping.Models;

namespace Domain.Shipping.Models;

public record ValidatedShipment
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    
    // Adresa a fost verificată și este corectă
    public required ShippingAddress ShippingAddress { get; init; }
    
    // Lista de produse a fost verificată (nu e goală)
    public IReadOnlyCollection<ShipmentLine> Lines { get; init; } = Array.Empty<ShipmentLine>();
}