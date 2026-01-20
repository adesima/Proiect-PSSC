using Domain.Shipping.Models;

namespace Shipping.Api.Messaging;

public record ShipmentManifestedMessage(
    Guid OrderId,
    Guid CustomerId,
    
    // Cel mai important: Codul AWB generat de tine
    AwbCode Awb,
    
    // Unde se livrează
    ShippingAddress ShippingAddress,          
    
    // Ce se află în pachet (ProductCode + Quantity)
    IReadOnlyCollection<ShipmentLine> Lines,

    // Cât a costat serviciul de livrare (calculat de tine)
    Money ShippingCost,                        
    
    // Momentul generării (ManifestedAt)
    DateTime DispatchedAt);