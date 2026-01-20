// // using System;
// // using System.Linq;
// // using Domain.Shipping.Models;
// //
// // namespace Domain.Shipping.Operations
// // {
// //     public class CalculateShippingCostOperation : DomainOperation<ValidatedShipment, object, CalculatedShipment>
// //     {
// //         public override CalculatedShipment Transform(ValidatedShipment shipment, object? _)
// //         {
// //             decimal basePrice = 10.0m;
// //             decimal volumePrice = shipment.Lines.Sum(line => line.Quantity * 2.0m);
// //             
// //             decimal finalCost = basePrice + volumePrice;
// //
// //             // Exemplu de logica extra
// //             if (shipment.DeliveryAddress.City.Equals("Timisoara", StringComparison.OrdinalIgnoreCase))
// //             {
// //                 finalCost -= 5.0m;
// //             }
// //             
// //             // Nu acceptam costuri negative
// //             if (finalCost < 0) finalCost = 0;
// //
// //             return new CalculatedShipment
// //             {
// //                 OrderId = shipment.OrderId,
// //                 CustomerId = shipment.CustomerId,
// //                 DeliveryAddress = shipment.DeliveryAddress,
// //                 Lines = shipment.Lines,
// //                 ShippingCost = finalCost
// //             };
// //         }
// //     }
// // }
//
//
// using System;
// using System.Linq;
// // using Domain.Shipping.Models;
// using static Domain.Shipping.Models.Shipment;
//
// namespace Domain.Shipping.Operations
// {
//     public class CalculateShippingCostOperation : DomainOperation<ValidatedShipment, IShipment>
//     {
//         public override IShipment Transform(ValidatedShipment input)
//         {
//             // --- Logica de Calcul ---
//             
//             // 1. Tarif de bază: 15 RON
//             decimal cost = 15.0m;
//
//             // 2. Adăugăm 2 RON pentru fiecare bucată de produs (simulăm greutatea/volumul)
//             // JSON-ul tău are "Quantity": 4, deci 4 * 2 = 8 RON
//             var totalQuantity = input.Lines.Sum(line => line.Quantity);
//             cost += totalQuantity * 2.0m;
//
//             // 3. Taxă extra pentru livrări în afara Bucureștiului sau Timișului (Exemplu)
//             if (input.Address.County != "Bucuresti" && input.Address.County != "Timis")
//             {
//                 cost += 5.0m; // Taxă de distanță
//             }
//
//             // Returnăm starea CALCULATĂ
//             return new CalculatedShipment(
//                 input.OrderId,
//                 input.CustomerId,
//                 input.Address,
//                 input.Lines,
//                 cost
//             );
//         }
//     }
// }


using Domain.Shipping.Models;

namespace Domain.Shipping.Operations;

public class CalculateShippingCostOperation 
    : DomainOperation<ValidatedShipment, object, CalculatedShipment>
{
    public override CalculatedShipment Transform(ValidatedShipment shipment, object? _)
    {
        // 1. Stabilim moneda (în mod normal ar veni din config, punem RON default)
        var currency = "RON";

        // 2. Regula de calcul (Simplificată, hardcodată ca la colegul tău)
        // Preț de pornire: 15.00 RON
        var baseRate = Money.Create(15.00m, currency);
        
        // Preț suplimentar per articol: 2.00 RON
        var perItemRate = Money.Create(2.00m, currency);

        // 3. Calculul efectiv
        var totalQuantity = shipment.Lines.Sum(line => line.Quantity);
        
        // Money știe să facă înmulțirea (Rate * int)
        var volumeCost = perItemRate * totalQuantity;

        // Money știe să facă adunarea (Base + Volume)
        var totalCost = baseRate + volumeCost;

        return new CalculatedShipment
        {
            OrderId = shipment.OrderId,
            CustomerId = shipment.CustomerId,
            ShippingAddress = shipment.ShippingAddress,
            Lines = shipment.Lines,
            ShippingCost = totalCost
        };
    }
}