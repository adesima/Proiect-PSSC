using Domain.Shipping.Models;

namespace Domain.Shipping.Operations;

public class ManifestShipmentOperation
    : DomainOperation<CalculatedShipment, object, ManifestedShipment>
{
    public override ManifestedShipment Transform(CalculatedShipment shipment, object? _)
    {
        // 1. Generăm codul AWB (Identificatorul unic)
        // În realitate, aici ai putea apela un API FanCourier/DPD
        // Pentru proiect, generăm un cod unic pe loc.
        var awbString = $"AWB-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        var awbCode = new AwbCode(awbString);

        // 2. Construim starea finală (ManifestedShipment)
        return new ManifestedShipment
        {
            // AWB-ul generat acum
            Awb = awbCode,
            
            // Copiem datele din starea anterioară
            OrderId = shipment.OrderId,
            CustomerId = shipment.CustomerId,
            ShippingAddress = shipment.ShippingAddress,
            Lines = shipment.Lines,
            
            // Costul calculat anterior
            ShippingCost = shipment.ShippingCost,
            
            // Setăm data curentă
            ManifestedAt = DateTime.Now
        };
    }
}