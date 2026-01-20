using Domain.Shipping.Models;

namespace Domain.Shipping.Models;

public record UnvalidatedShipment
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    
    // Adresa brută primită, încă nevalidată
    public required ShippingAddress ShippingAddress { get; init; }
    
    // Lista brută de produse
    public IReadOnlyCollection<ShipmentLine> Lines { get; init; } = Array.Empty<ShipmentLine>();
}