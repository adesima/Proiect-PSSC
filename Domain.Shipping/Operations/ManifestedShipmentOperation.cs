using Domain.Shipping.Models;

namespace Domain.Shipping.Operations;

public class ManifestShipmentOperation
    : DomainOperation<CalculatedShipment, object, ManifestedShipment>
{
    public override ManifestedShipment Transform(CalculatedShipment shipment, object? _)
    {
        // Generez codul AWB
        var awbString = $"AWB-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        var awbCode = new AwbCode(awbString);

        // Construiesc starea finala (ManifestedShipment)
        return new ManifestedShipment
        {
            // AWB-ul generat acum
            Awb = awbCode,
            
            // Copiez datele din starea anterioara
            OrderId = shipment.OrderId,
            CustomerId = shipment.CustomerId,
            ShippingAddress = shipment.ShippingAddress,
            Lines = shipment.Lines,
            
            // Costul calculat anterior
            ShippingCost = shipment.ShippingCost,
            
            // Setez data curenta
            ManifestedAt = DateTime.Now
        };
    }
}