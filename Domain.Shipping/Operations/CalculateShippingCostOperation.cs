using System;
using System.Linq;
using Domain.Shipping.Models;

namespace Domain.Shipping.Operations
{
    public class CalculateShippingCostOperation : DomainOperation<ValidatedShipment, object, CalculatedShipment>
    {
        public override CalculatedShipment Transform(ValidatedShipment shipment, object? _)
        {
            decimal basePrice = 10.0m;
            decimal volumePrice = shipment.Lines.Sum(line => line.Quantity * 2.0m);
            
            decimal finalCost = basePrice + volumePrice;

            // Exemplu de logica extra
            if (shipment.DeliveryAddress.City.Equals("Timisoara", StringComparison.OrdinalIgnoreCase))
            {
                finalCost -= 5.0m;
            }
            
            // Nu acceptam costuri negative
            if (finalCost < 0) finalCost = 0;

            return new CalculatedShipment
            {
                OrderId = shipment.OrderId,
                CustomerId = shipment.CustomerId,
                DeliveryAddress = shipment.DeliveryAddress,
                Lines = shipment.Lines,
                ShippingCost = finalCost
            };
        }
    }
}