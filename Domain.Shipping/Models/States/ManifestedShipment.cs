using System;
using System.Collections.Generic;

namespace Domain.Shipping.Models
{
    public record ManifestedShipment
    {
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public required ShippingAddress DeliveryAddress { get; init; }
        public IReadOnlyCollection<ShipmentLine> Lines { get; init; } = new List<ShipmentLine>();
        public decimal ShippingCost { get; init; }
        
        // Câmpul nou: AWB-ul generat
        public required AwbCode Awb { get; init; }
    }
}