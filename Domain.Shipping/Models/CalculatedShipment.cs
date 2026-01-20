using System;
using System.Collections.Generic;

namespace Domain.Shipping.Models
{
    public record CalculatedShipment
    {
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public required ShippingAddress ShippingAddress { get; init; }
        public IReadOnlyCollection<ShipmentLine> Lines { get; init; } = Array.Empty<ShipmentLine>();
        
        public required Money ShippingCost { get; init; }
    }
}