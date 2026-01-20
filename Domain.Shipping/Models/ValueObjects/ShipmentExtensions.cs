// using System;
// using static Domain.Shipping.Models.Shipment;
//
// namespace Domain.Shipping.Models
// {
//     public static class ShipmentExtensions
//     {
//         public static IShipmentEvent ToEvent(this IShipment shipment)
//         {
//             return shipment switch
//             {
//                 // Cazul Fericit: Livrarea a reușit, avem AWB
//                 ManifestedShipment manifested => new ShipmentManifestedEvent(
//                     manifested.Awb.Value, 
//                     manifested.OrderId, 
//                     manifested.ShippingCost),
//
//                 // Cazul Nefericit: Validarea a eșuat
//                 InvalidShipment invalid => new ShipmentFailedEvent(invalid.Reason),
//
//                 // Cazul în care workflow-ul s-a oprit la mijloc (nu ar trebui să se întâmple)
//                 _ => new ShipmentFailedEvent("Unknown state generated during processing.")
//             };
//         }
//     }
// }