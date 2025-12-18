using Domain.Invoicing.Models;

namespace Domain.Invoicing.Operations;

public class ValidateInvoiceOperation
    : DomainOperation<UnvalidatedInvoice, object, ValidatedInvoice>
{
    public override ValidatedInvoice Transform(UnvalidatedInvoice invoice, object? _)
    {
        if (invoice.Lines == null || invoice.Lines.Count == 0)
            throw new ArgumentException("Invoice must contain at least one line.");

        foreach (var line in invoice.Lines)
        {
            if (line.Quantity <= 0)
                throw new ArgumentException("Line quantity must be positive.");

            if (line.UnitPrice.Amount < 0)
                throw new ArgumentException("Unit price cannot be negative.");
        }

        return new ValidatedInvoice
        {
            OrderId = invoice.OrderId,
            CustomerId = invoice.CustomerId,
            BillingAddress = invoice.BillingAddress,
            Lines = invoice.Lines
        };
    }
}
