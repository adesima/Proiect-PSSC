// // using System;
// // using Domain.Shipping.Models;
// //
// // // clasa de baza operartii
// //
// // namespace Domain.Shipping.Operations
// // {
// //     // Transformam UnvalidatedShipment -> ValidatedShipment
// //     // Folosim 'object' pentru context deoarece nu avem nevoie de date externe momentan
// //     public class ValidateShipmentOperation : DomainOperation<UnvalidatedShipment, object, ValidatedShipment>
// //     {
// //         public override ValidatedShipment Transform(UnvalidatedShipment shipment, object? _)
// //         {
// //             // 1. Verificam sa avem produse de livrat
// //             if (shipment.Lines == null || shipment.Lines.Count == 0)
// //                 throw new ArgumentException("Shipment must contain at least one item.");
// //
// //             // 2. Verificam cantitatile
// //             foreach (var line in shipment.Lines)
// //             {
// //                 if (line.Quantity <= 0)
// //                     throw new ArgumentException($"Invalid quantity for product {line.ProductCode}. Must be positive.");
// //             }
// //
// //             // 3. Verificam adresa (chiar daca e Value Object, ne asiguram ca nu e null)
// //             if (shipment.DeliveryAddress == null)
// //                 throw new ArgumentNullException(nameof(shipment.DeliveryAddress));
// //
// //             // Daca am ajuns aici, totul e corect. Returnam starea validata.
// //             return new ValidatedShipment
// //             {
// //                 OrderId = shipment.OrderId,
// //                 CustomerId = shipment.CustomerId,
// //                 DeliveryAddress = shipment.DeliveryAddress,
// //                 Lines = shipment.Lines
// //             };
// //         }
// //     }
// // }
//
//
// using System;
// using System.Linq;
// using System.Text;
// using Domain.Shipping.Models;
// using static Domain.Shipping.Models.Shipment;
//
// namespace Domain.Shipping.Operations
// {
//     public class ValidateShipmentOperation : DomainOperation<UnvalidatedShipment, IShipment>
//     {
//         public override IShipment Transform(UnvalidatedShipment input)
//         {
//             var errors = new StringBuilder();
//
//             // 1. Validare Adresă (bazat pe JSON-ul tău)
//             if (input.Address == null)
//             {
//                 errors.AppendLine("Shipping address is missing.");
//             }
//             else
//             {
//                 if (string.IsNullOrWhiteSpace(input.Address.City))
//                     errors.AppendLine("City is required.");
//                 if (string.IsNullOrWhiteSpace(input.Address.Street))
//                     errors.AppendLine("Street is required.");
//                 if (string.IsNullOrWhiteSpace(input.Address.County))
//                     errors.AppendLine("County is required.");
//                 if (string.IsNullOrWhiteSpace(input.Address.PostalCode))
//                     errors.AppendLine("PostalCode is required.");
//             }
//
//             // 2. Validare Produse
//             if (input.Lines == null || input.Lines.Count == 0)
//             {
//                 errors.AppendLine("Shipment must contain at least one item.");
//             }
//
//             // A. Dacă avem erori, returnăm starea INVALIDĂ
//             if (errors.Length > 0)
//             {
//                 return new InvalidShipment(input, errors.ToString());
//             }
//
//             // B. Dacă totul e OK, returnăm starea VALIDATĂ
//             return new ValidatedShipment(
//                 input.OrderId,
//                 input.CustomerId,
//                 input.Address,
//                 input.Lines
//             );
//         }
//     }
// }


using Domain.Shipping.Models;

namespace Domain.Shipping.Operations;

public class ValidateShipmentOperation
    : DomainOperation<UnvalidatedShipment, object, ValidatedShipment>
{
    public override ValidatedShipment Transform(UnvalidatedShipment shipment, object? _)
    {
        // 1. Validare Produse
        if (shipment.Lines == null || shipment.Lines.Count == 0)
            throw new ArgumentException("Shipment must contain at least one item.");

        foreach (var line in shipment.Lines)
        {
            if (line.Quantity <= 0)
                throw new ArgumentException("Shipment line quantity must be positive.");
            
            // NOTĂ: Nu validăm UnitPrice aici, curierul nu știe prețul produselor.
        }

        // 2. Validare Adresă (Critic pentru curierat)
        if (shipment.ShippingAddress == null)
            throw new ArgumentException("Shipping address is required.");

        // Verificăm dacă adresa are datele minime necesare
        if (string.IsNullOrWhiteSpace(shipment.ShippingAddress.City) || 
            string.IsNullOrWhiteSpace(shipment.ShippingAddress.Street))
        {
            throw new ArgumentException("Shipping address must contain at least City and Street.");
        }

        // 3. Returnăm starea Validată
        return new ValidatedShipment
        {
            OrderId = shipment.OrderId,
            CustomerId = shipment.CustomerId,
            ShippingAddress = shipment.ShippingAddress,
            Lines = shipment.Lines
        };
    }
}