using System;
using Domain.Shipping.Models;

namespace Domain.Shipping.Operations
{
    public class GenerateAwbOperation : DomainOperation<CalculatedShipment, object, ManifestedShipment>
    {
        public override ManifestedShipment Transform(CalculatedShipment shipment, object? _)
        {   
            var awbString = $"AWB-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            var awb = AwbCode.Create(awbString);

            return new ManifestedShipment
            {
                OrderId = shipment.OrderId,
                CustomerId = shipment.CustomerId,
                DeliveryAddress = shipment.DeliveryAddress,
                Lines = shipment.Lines,
                ShippingCost = shipment.ShippingCost,
                Awb = awb
            };
        }
    }
}