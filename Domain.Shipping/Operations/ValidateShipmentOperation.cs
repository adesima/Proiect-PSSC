using System;
using Domain.Shipping.Models;

namespace Domain.Shipping.Operations
{
    // Transformam UnvalidatedShipment -> ValidatedShipment
    // Folosim 'object' pentru context deoarece nu avem nevoie de date externe momentan
    public class ValidateShipmentOperation : DomainOperation<UnvalidatedShipment, object, ValidatedShipment>
    {
        public override ValidatedShipment Transform(UnvalidatedShipment shipment, object? _)
        {
            // 1. Verificam sa avem produse de livrat
            if (shipment.Lines == null || shipment.Lines.Count == 0)
                throw new ArgumentException("Shipment must contain at least one item.");

            // 2. Verificam cantitatile
            foreach (var line in shipment.Lines)
            {
                if (line.Quantity <= 0)
                    throw new ArgumentException($"Invalid quantity for product {line.ProductCode}. Must be positive.");
            }

            // 3. Verificam adresa (chiar daca e Value Object, ne asiguram ca nu e null)
            if (shipment.DeliveryAddress == null)
                throw new ArgumentNullException(nameof(shipment.DeliveryAddress));

            // Daca am ajuns aici, totul e corect. Returnam starea validata.
            return new ValidatedShipment
            {
                OrderId = shipment.OrderId,
                CustomerId = shipment.CustomerId,
                DeliveryAddress = shipment.DeliveryAddress,
                Lines = shipment.Lines
            };
        }
    }
}