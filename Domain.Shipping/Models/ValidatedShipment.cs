using Domain.Shipping.Models;

namespace Domain.Shipping.Models;

public record ValidatedShipment
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    
    // Adresa a fost verificata si este corecta
    public required ShippingAddress ShippingAddress { get; init; }
    
    // Lista de produse a fost verificata 
    public IReadOnlyCollection<ShipmentLine> Lines { get; init; } = Array.Empty<ShipmentLine>();
}