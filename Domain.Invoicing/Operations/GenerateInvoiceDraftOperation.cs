using Domain.Invoicing.Models;

namespace Domain.Invoicing.Operations;

public class GenerateInvoiceDraftOperation
    : DomainOperation<GenerateInvoiceDraftCommand, object, UnvalidatedInvoice>
{
    public override UnvalidatedInvoice Transform(GenerateInvoiceDraftCommand command, object? _)
    {
        return new UnvalidatedInvoice
        {
            OrderId = command.OrderId,
            CustomerId = command.CustomerId,
            BillingAddress = command.BillingAddress,
            Lines = command.Lines
        };
    }
}
