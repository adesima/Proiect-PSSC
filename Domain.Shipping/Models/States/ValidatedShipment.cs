using System;
using System.Collections.Generic;

namespace Domain.Shipping.Models
{
    public record ValidatedShipment
    {
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public required ShippingAddress DeliveryAddress { get; init; }
        public IReadOnlyCollection<ShipmentLine> Lines { get; init; } = new List<ShipmentLine>();
    }
}