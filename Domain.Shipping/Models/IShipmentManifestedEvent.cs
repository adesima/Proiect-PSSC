namespace Domain.Shipping.Models;

public interface IShipmentManifestedEvent
{
    Guid OrderId { get; }
    Guid CustomerId { get; }
    ShippingAddress ShippingAddress { get; init; }
    
    // Lista de linii ale expedierii (ProductCode + Quantity)
    IReadOnlyCollection<ShipmentLine> Lines { get; }
    
    // Costul calculat al transportului
    Money ShippingCost { get; }
    
    // Codul AWB generat 
    AwbCode Awb { get; }
    
    // Data și ora la care s-a generat AWB-ul
    DateTime ManifestedAt { get; }
}

public record ShipmentManifestedEvent(
    Guid OrderId,
    Guid CustomerId,
    ShippingAddress ShippingAddress,
    IReadOnlyCollection<ShipmentLine> Lines,
    Money ShippingCost,
    AwbCode Awb,
    DateTime ManifestedAt
) : IShipmentManifestedEvent;
