// using System;
//
// namespace Domain.Shipping.Models
// {
//     // Interfața comună pentru rezultatul workflow-ului
//     public interface IShipmentEvent { }
//
//     // Evenimentul de Succes: Am generat AWB-ul
//     public record ShipmentManifestedEvent : IShipmentEvent
//     {
//         public string Awb { get; }
//         public Guid OrderId { get; }
//         public decimal ShippingCost { get; }
//         public DateTime ManifestedAt { get; }
//
//         public ShipmentManifestedEvent(string awb, Guid orderId, decimal shippingCost)
//         {
//             Awb = awb;
//             OrderId = orderId;
//             ShippingCost = shippingCost;
//             ManifestedAt = DateTime.Now;
//         }
//     }
//
//     // Evenimentul de Eșec: Ceva nu a mers (validare eșuată, etc.)
//     public record ShipmentFailedEvent : IShipmentEvent
//     {
//         public string Reason { get; }
//
//         public ShipmentFailedEvent(string reason)
//         {
//             Reason = reason;
//         }
//     }
// }