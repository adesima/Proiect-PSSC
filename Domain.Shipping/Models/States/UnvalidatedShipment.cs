// using System;
// using System.Collections.Generic;
//
// namespace Domain.Shipping.Models
// {
//     public record UnvalidatedShipment
//     {
//         public Guid OrderId { get; init; }
//         public Guid CustomerId { get; init; }
//         public required ShippingAddress DeliveryAddress { get; init; }
//         public IReadOnlyCollection<ShipmentLine> Lines { get; init; } = new List<ShipmentLine>();
//     }
//
//     // Definim si linia de produs simpla (Value Object intern)
//     public record ShipmentLine(string ProductCode, int Quantity);
// }

