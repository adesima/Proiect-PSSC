namespace Domain.Shipping.Models;

public class ProcessShipmentCommand
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public ShippingAddress ShippingAddress { get; set; } = default!;
    
    // Lista de produse 
    public IReadOnlyCollection<ShipmentLine> Lines { get; set; } = Array.Empty<ShipmentLine>();
}