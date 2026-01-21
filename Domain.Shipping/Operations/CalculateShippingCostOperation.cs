using Domain.Shipping.Models;

namespace Domain.Shipping.Operations;

public class CalculateShippingCostOperation 
    : DomainOperation<ValidatedShipment, object, CalculatedShipment>
{
    public override CalculatedShipment Transform(ValidatedShipment shipment, object? _)
    {
        // Stabilim moneda (RON default)
        var currency = "RON";

        // Regula de calcul 
        // Preț de pornire: 15.00 RON
        var baseRate = Money.Create(15.00m, currency);
        
        // Pret suplimentar per articol: 2.00 RON
        var perItemRate = Money.Create(2.00m, currency);

        // Calculul efectiv
        var totalQuantity = shipment.Lines.Sum(line => line.Quantity);
        
        // Inmultire din Money
        var volumeCost = perItemRate * totalQuantity;

        // Adunare din Money
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