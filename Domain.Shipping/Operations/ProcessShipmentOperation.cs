using Domain.Shipping.Models;

namespace Domain.Shipping.Operations;

public class ProcessShipmentOperation
    : DomainOperation<ProcessShipmentCommand, object, UnvalidatedShipment>
{
    public override UnvalidatedShipment Transform(ProcessShipmentCommand command, object? _)
    {
        return new UnvalidatedShipment
        {
            OrderId = command.OrderId,
            CustomerId = command.CustomerId,
            ShippingAddress = command.ShippingAddress,
            Lines = command.Lines
        };
    }
}