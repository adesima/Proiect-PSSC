using Domain.Shipping.Models;

namespace Domain.Shipping.Operations;

public class ValidateShipmentOperation
    : DomainOperation<UnvalidatedShipment, object, ValidatedShipment>
{
    public override ValidatedShipment Transform(UnvalidatedShipment shipment, object? _)
    {
        // Validare Produse
        if (shipment.Lines == null || shipment.Lines.Count == 0)
            throw new ArgumentException("Shipment must contain at least one item.");

        foreach (var line in shipment.Lines)
        {
            if (line.Quantity <= 0)
                throw new ArgumentException("Shipment line quantity must be positive.");
        }

        // Validare adresa de livrare
        if (shipment.ShippingAddress == null)
            throw new ArgumentException("Shipping address is required.");

        // Verificare daca adresa are datele minime necesare
        if (string.IsNullOrWhiteSpace(shipment.ShippingAddress.City) || 
            string.IsNullOrWhiteSpace(shipment.ShippingAddress.Street))
        {
            throw new ArgumentException("Shipping address must contain at least City and Street.");
        }

        // Returnez starea validata
        return new ValidatedShipment
        {
            OrderId = shipment.OrderId,
            CustomerId = shipment.CustomerId,
            ShippingAddress = shipment.ShippingAddress,
            Lines = shipment.Lines
        };
    }
}