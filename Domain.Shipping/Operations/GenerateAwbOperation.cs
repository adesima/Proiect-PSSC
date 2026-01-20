// // using System;
// // using Domain.Shipping.Models;
// //
// // namespace Domain.Shipping.Operations
// // {
// //     public class GenerateAwbOperation : DomainOperation<CalculatedShipment, object, ManifestedShipment>
// //     {
// //         public override ManifestedShipment Transform(CalculatedShipment shipment, object? _)
// //         {   
// //             var awbString = $"AWB-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
// //             var awb = AwbCode.Create(awbString);
// //
// //             return new ManifestedShipment
// //             {
// //                 OrderId = shipment.OrderId,
// //                 CustomerId = shipment.CustomerId,
// //                 DeliveryAddress = shipment.DeliveryAddress,
// //                 Lines = shipment.Lines,
// //                 ShippingCost = shipment.ShippingCost,
// //                 Awb = awb
// //             };
// //         }
// //     }
// // }
//
//
//
// using System;
// using Domain.Shipping.Models;
// // using Domain.Shipping.Models.ValueObjects;
// using static Domain.Shipping.Models.Shipment;
//
// namespace Domain.Shipping.Operations
// {
//     public class GenerateAwbOperation : DomainOperation<CalculatedShipment, IShipment>
//     {
//         public override IShipment Transform(CalculatedShipment input)
//         {
//             // Generăm un cod AWB unic
//             // Format: AWB-{8 caractere random}
//             var awbString = $"AWB-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
//             var awb = new AwbCode(awbString);
//
//             // Returnăm starea MANIFESTATĂ (Finală)
//             return new ManifestedShipment(
//                 input.OrderId,
//                 input.CustomerId,
//                 input.Address,
//                 input.Lines,
//                 input.ShippingCost,
//                 awb
//             );
//         }
//     }
// }